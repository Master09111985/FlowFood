using FlowFood.Data;
using FlowFood.Models;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

namespace FlowFood.Repositorio
{
  public class ClienteRepositorio : IClienteRepositorio
  {
    private readonly DataContext _context;

    public ClienteRepositorio(DataContext context)
    {
        _context = context;
    }

    // Consultas
    public async Task<ICollection<Cliente>> GetClientesAsync()
    {
      return await _context.Clientes.OrderBy(c => c.Nombre).ToListAsync();
    }
    public async Task<Cliente> GetClienteAsync(int id)
    {
      return await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
    }
    public async Task<Cliente> GetClienteXTelefonoAsync(string telefono)
    {
      return await _context.Clientes.FirstOrDefaultAsync(c => c.Telefono.Trim() == telefono.Trim());
    }


    // Para validaciones
    public async Task<bool> ExisteClienteAsync(int id)
    {
      return await _context.Clientes.AnyAsync(c => c.Id == id);
    }
    public async Task<bool> ExisteClienteXTelefonoAsync(string telefono)
    {
      bool valor = await _context.Clientes.AnyAsync(c => c.Telefono.ToLower().Trim() == telefono);
      return valor;
    }


    // Para el CRUD
    public async Task<bool> CrearClienteAsync(Cliente cliente)
    {
      _context.Clientes.Add(cliente);
      return await GuardarAsync();
    }
    public async Task<bool> ActualizarClienteAsync(Cliente cliente)
    {
      _context.Clientes.Update(cliente);
      return await GuardarAsync();
    }
    public async Task<bool> BorrarClienteAsync(Cliente cliente)
    {
      _context.Clientes.Remove(cliente);
      return await GuardarAsync();
    }

    public async Task<bool> GuardarAsync()
    {
      return await _context.SaveChangesAsync() > 0 ? true : false;
    }
  }
}
