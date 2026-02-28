using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesApp.Api.Models;
using NotesApp.Api.Repositories;

namespace NotesApp.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly INoteRepository _noteRepository;

    public NotesController(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var notes = await _noteRepository.GetAllAsync(GetUserId());
        return Ok(notes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var note = await _noteRepository.GetByIdAsync(id, GetUserId());
        if (note == null) return NotFound();
        return Ok(note);
    }

    [HttpPost]
    public async Task<IActionResult> Create(NoteCreateRequest request)
    {
        var note = new Note
        {
            Title = request.Title,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UserId = GetUserId()
        };

        var id = await _noteRepository.CreateAsync(note);
        note.Id = id;
        return CreatedAtAction(nameof(GetById), new { id = note.Id }, note);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, NoteUpdateRequest request)
    {
        var note = await _noteRepository.GetByIdAsync(id, GetUserId());
        if (note == null) return NotFound();

        note.Title = request.Title;
        note.Content = request.Content;
        note.UpdatedAt = DateTime.UtcNow;

        var result = await _noteRepository.UpdateAsync(note);
        if (!result) return StatusCode(500, "Error updating note");

        return Ok(note);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _noteRepository.DeleteAsync(id, GetUserId());
        if (!result) return NotFound();
        return NoContent();
    }
}
