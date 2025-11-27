using Microsoft.AspNetCore.Mvc;
using OrdersStatusMicroservice.Data;
using OrdersStatusMicroservice.Models;

namespace OrdersStatusMicroservice.Controllers;

[ApiController]
[Route("[controller]")]
public class StatusController : ControllerBase
{
    // GET /status
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(FakeDb.Statuses);
    }

    // PUT /status
    [HttpPut]
    public IActionResult Create([FromBody] OrderStatus status)
    {
        if (string.IsNullOrWhiteSpace(status.Name))
            return BadRequest("Название статуса не может быть пустым");

        status.Id = FakeDb.Statuses.Max(x => x.Id) + 1;
        FakeDb.Statuses.Add(status);

        return Ok(status);
    }

    // POST /status/{id}
    [HttpPost("{id}")]
    public IActionResult Update(int id, [FromBody] OrderStatus status)
    {
        var existing = FakeDb.Statuses.FirstOrDefault(x => x.Id == id);
        if (existing == null)
            return NotFound("Статус не найден");

        existing.Name = status.Name;
        return Ok(existing);
    }

    // DELETE /status/{id}
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var existing = FakeDb.Statuses.FirstOrDefault(x => x.Id == id);
        if (existing == null)
            return NotFound("Статус не найден");

        FakeDb.Statuses.Remove(existing);
        return Ok();
    }
}
