using Microsoft.AspNetCore.Mvc;
using MiniForestApp.Models; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 
using Microsoft.EntityFrameworkCore; 

[ApiController]
[Route("[controller]")]
public class FocusController : ControllerBase
{
    private readonly MiniForestDbContext _context;

    public FocusController(MiniForestDbContext context) 
    {
        _context = context;
    }

    // GET /Focus/completed - Bitmiş oturumları listele
    [HttpGet("completed")]
    public async Task<IActionResult> GetCompletedSessions()
    {
        try
        {
            var completed = await _context.FocusSessions
                .Where(x => x.IsCompleted)
                .OrderByDescending(x => x.StartTime)
                .Select(x => new FocusSessionDto(x))
                .ToListAsync();

            return Ok(Response<List<FocusSessionDto>>.Successful(completed));
        }
        catch (Exception ex)
        {
            return BadRequest(Response<List<FocusSessionDto>>.Fail(ex.Message));
        }
    }
    
    // GET /Focus - Tüm oturumları (devam edenler dahil) listele
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _context.FocusSessions
                                       .OrderByDescending(s => s.StartTime)
                                       .Select(x => new FocusSessionDto(x))
                                       .ToListAsync();

            return Ok(Response<List<FocusSessionDto>>.Successful(result));
        }
        catch (Exception ex)
        {
            return BadRequest(Response<List<FocusSessionDto>>.Fail(ex.Message));
        }
    }

    // POST /Focus/start - Oturumu Başlat (ÇALIŞMA SORUNU GİDERİLDİ: [FromBody] EKLENDİ)
    [HttpPost("start")]
    public async Task<IActionResult> StartSession([FromBody] StartFocusDto dto)
    {
        try
        {
            if (dto.DurationMinutes <= 0)
                return BadRequest(Response<FocusSessionDto>.Fail("Süre 0'dan büyük olmalıdır."));

            var session = new FocusSession()
            {
                DurationMinutes = dto.DurationMinutes,
                StartTime = DateTime.Now,
                IsCompleted = false
            };

            _context.FocusSessions.Add(session);
            await _context.SaveChangesAsync();

            return Ok(Response<FocusSessionDto>.Successful(new FocusSessionDto(session)));
        }
        catch (Exception ex)
        {
            return BadRequest(Response<FocusSessionDto>.Fail(ex.Message));
        }
    }

    // POST /Focus/finish/{id} - Oturumu Bitir (ÇALIŞMA SORUNU VE ROTA ÇAKIŞMASI GİDERİLDİ)
    [HttpPost("finish/{id:int}")] // Rota çakışmasını engeller
    public async Task<IActionResult> FinishSession(int id)
    {
        try
        {
            var found = await _context.FocusSessions.FindAsync(id);
            
            if (found == null)
                return NotFound(Response<FocusSessionDto>.Fail("Oturum bulunamadı."));

            if (found.IsCompleted)
                return BadRequest(Response<FocusSessionDto>.Fail("Oturum zaten tamamlanmış."));

            // GERÇEK SÜRE HESAPLAMA MANTIĞI
            DateTime now = DateTime.Now;
            TimeSpan actualDuration = now - found.StartTime;
            int completedMinutes = (int)Math.Round(actualDuration.TotalMinutes);
            
            if (completedMinutes < 0) completedMinutes = 0;

            found.EndTime = now;
            found.IsCompleted = true;
            found.DurationMinutes = completedMinutes; // GERÇEK SÜREYİ KAYDET

            _context.FocusSessions.Update(found);
            await _context.SaveChangesAsync();

            return Ok(Response<FocusSessionDto>.Successful(new FocusSessionDto(found)));
        }
        catch (Exception ex)
        {
            return BadRequest(Response<FocusSessionDto>.Fail(ex.Message));
        }
    }

    // GET /Focus/today - Bugün çalışılan toplam süreyi getirir
    [HttpGet("today")]
    public async Task<IActionResult> GetTodaySummary()
    {
        try
        {
            var today = DateTime.Today;

            var total = await _context.FocusSessions
                .Where(x => x.StartTime.Date == today && x.IsCompleted)
                .SumAsync(x => x.DurationMinutes); 

            var dto = new TodaySummaryDto(today, total);

            return Ok(Response<TodaySummaryDto>.Successful(dto));
        }
        catch (Exception ex)
        {
            return BadRequest(Response<TodaySummaryDto>.Fail(ex.Message));
        }
    }
    
    // GET /Focus/{id} - Tek bir oturumu getirir (ROTA ÇAKIŞMASI GİDERİLDİ)
    [HttpGet("{id:int}")] // Rota çakışmasını engeller
    public async Task<IActionResult> GetSession(int id)
    {
        try
        {
            var found = await _context.FocusSessions.FindAsync(id);
            
            if (found == null)
                return NotFound(Response<FocusSessionDto>.Fail("Oturum bulunamadı."));

            return Ok(Response<FocusSessionDto>.Successful(new FocusSessionDto(found)));
        }
        catch (Exception ex)
        {
            return BadRequest(Response<FocusSessionDto>.Fail(ex.Message));
        }
    }
}