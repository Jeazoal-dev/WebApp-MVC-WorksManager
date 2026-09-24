using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Data
{
    // Carga datos de prueba solo si la base está vacía.
    // Útil para demos y para probar filtros/estados sin meter datos a mano.
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext db)
        {
            // Si ya hay obras, no toca nada.
            if (await db.Works.AnyAsync()) return;

            var today = DateTime.Today;

            // --------------------------------------------------------------
            // TRABAJADORES
            // --------------------------------------------------------------
            var workers = new List<Worker>
            {
                new() { Name = "Juan Pérez",     Document = "73591158", Phone = "924431271", Bank = "Interbank", AccountNumber = "1231233123", Status = "Active" },
                new() { Name = "Ana López",       Document = "73592352", Phone = "924431272", Bank = "BCP",       AccountNumber = "1231233124", Status = "Active" },
                new() { Name = "Luis Ruiz",       Document = "72522312", Phone = "921431222", Bank = "Scotiabank",AccountNumber = "1231233125", Status = "Active" },
                new() { Name = "María Torres",    Document = "45123456", Phone = "987654321", Bank = "BBVA",      AccountNumber = "1231233126", Status = "Active" },
                new() { Name = "Carlos Mendoza",  Document = "40987654", Phone = "998877665", Bank = "Interbank", AccountNumber = "1231233127", Status = "Active" },
                new() { Name = "Pedro Ramírez",   Document = "44556677", Phone = "911223344", Bank = "BCP",       AccountNumber = "1231233128", Status = "Inactive" },
                new() { Name = "Sofía Vargas",    Document = "47889911", Phone = "977889900", Bank = "Scotiabank",AccountNumber = "1231233129", Status = "Active" },
                new() { Name = "Diego Flores",    Document = "43221100", Phone = "966778899", Bank = "BBVA",      AccountNumber = "1231233130", Status = "Inactive" },
                new() { Name = "Lucía Castro",    Document = "48990011", Phone = "955667788", Bank = "Interbank", AccountNumber = "1231233131", Status = "Active" },
                new() { Name = "Jorge Salinas",   Document = "41223344", Phone = "944556677", Bank = "BCP",       AccountNumber = "1231233132", Status = "Active" },
            };
            db.Workers.AddRange(workers);

            // --------------------------------------------------------------
            // OBRAS
            // --------------------------------------------------------------
            var works = new List<Work>
            {
                new() { Name = "Puente San Martín",  Client = "Municipalidad de Lima", StartDate = today.AddDays(-90), EndDate = today.AddDays(30),  ContractAmount = 100_000m, CollectedAmount = 40_000m, Status = WorkStatus.InProgress, Notes = "Obra principal del trimestre" },
                new() { Name = "Edificio Aurora",     Client = "Inmobiliaria Aurora SAC", StartDate = today.AddDays(-180), EndDate = today.AddDays(-10), ContractAmount = 250_000m, CollectedAmount = 250_000m, Status = WorkStatus.Finished,  Notes = "Entregada sin observaciones" },
                new() { Name = "Casa Los Álamos",     Client = "Familia Rodríguez",    StartDate = today.AddDays(-30),  EndDate = today.AddDays(60),  ContractAmount = 80_000m,  CollectedAmount = 20_000m, Status = WorkStatus.InProgress, Notes = "Cliente pidió ampliación" },
                new() { Name = "Remodelación Oficina",Client = "Tech Corp SAC",       StartDate = today.AddDays(-15),  EndDate = today.AddDays(45),  ContractAmount = 45_000m,  CollectedAmount = 15_000m, Status = WorkStatus.InProgress },
                new() { Name = "Pista Los Olivos",    Client = "Municipalidad de SJL",StartDate = today.AddDays(-120), EndDate = today.AddDays(-30), ContractAmount = 180_000m, CollectedAmount = 180_000m, Status = WorkStatus.Finished },
                new() { Name = "Almacén Industrial",  Client = "Logística Perú SAC",  StartDate = today.AddDays(-60),  EndDate = today.AddDays(90),  ContractAmount = 320_000m, CollectedAmount = 100_000m, Status = WorkStatus.Paused,    Notes = "Pausado por falta de materiales" },
                new() { Name = "Colegio San Marcos",  Client = "UGEL Norte",          StartDate = today.AddDays(-200), EndDate = today.AddDays(-60), ContractAmount = 150_000m, CollectedAmount = 150_000m, Status = WorkStatus.Finished },
                new() { Name = "Plaza Comercial",     Client = "Grupo Inversor SAC",  StartDate = today.AddDays(-45),  EndDate = today.AddDays(120), ContractAmount = 500_000m, CollectedAmount = 75_000m, Status = WorkStatus.InProgress },
                new() { Name = "Vivienda Unifamiliar",Client = "Sr. Gutiérrez",       StartDate = today.AddDays(-10),  EndDate = today.AddDays(80),  ContractAmount = 60_000m,  CollectedAmount = 0m,      Status = WorkStatus.InProgress },
                new() { Name = "Local Comercial",     Client = "Emprendedores SAC",   StartDate = today.AddDays(-90),  EndDate = today.AddDays(-15), ContractAmount = 35_000m,  CollectedAmount = 10_000m, Status = WorkStatus.Cancelled, Notes = "Cliente desistió del proyecto" },
            };
            db.Works.AddRange(works);

            await db.SaveChangesAsync();

            // --------------------------------------------------------------
            // ASIGNACIONES (WorkWorker)
            // --------------------------------------------------------------
            var assignments = new List<WorkWorker>
            {
                // Puente San Martín (obra activa grande)
                new() { WorkId = works[0].Id, WorkerId = workers[0].Id, AgreedAmount = 15_000m },
                new() { WorkId = works[0].Id, WorkerId = workers[1].Id, AgreedAmount = 12_000m },
                new() { WorkId = works[0].Id, WorkerId = workers[4].Id, AgreedAmount = 8_000m  },

                // Edificio Aurora (finalizada, todo pagado)
                new() { WorkId = works[1].Id, WorkerId = workers[2].Id, AgreedAmount = 45_000m },
                new() { WorkId = works[1].Id, WorkerId = workers[3].Id, AgreedAmount = 30_000m },

                // Casa Los Álamos
                new() { WorkId = works[2].Id, WorkerId = workers[0].Id, AgreedAmount = 10_000m },
                new() { WorkId = works[2].Id, WorkerId = workers[6].Id, AgreedAmount = 9_000m  },

                // Remodelación Oficina
                new() { WorkId = works[3].Id, WorkerId = workers[7].Id, AgreedAmount = 6_000m  },

                // Pista Los Olivos (finalizada)
                new() { WorkId = works[4].Id, WorkerId = workers[2].Id, AgreedAmount = 22_000m },
                new() { WorkId = works[4].Id, WorkerId = workers[8].Id, AgreedAmount = 18_000m },

                // Almacén Industrial (pausada)
                new() { WorkId = works[5].Id, WorkerId = workers[4].Id, AgreedAmount = 30_000m },
                new() { WorkId = works[5].Id, WorkerId = workers[9].Id, AgreedAmount = 25_000m },
                new() { WorkId = works[5].Id, WorkerId = workers[0].Id, AgreedAmount = 15_000m },

                // Colegio San Marcos (finalizada)
                new() { WorkId = works[6].Id, WorkerId = workers[1].Id, AgreedAmount = 25_000m },
                new() { WorkId = works[6].Id, WorkerId = workers[3].Id, AgreedAmount = 20_000m },

                // Plaza Comercial (activa grande)
                new() { WorkId = works[7].Id, WorkerId = workers[2].Id, AgreedAmount = 40_000m },
                new() { WorkId = works[7].Id, WorkerId = workers[5].Id, AgreedAmount = 35_000m },

                // Vivienda Unifamiliar (sin pagos aún)
                new() { WorkId = works[8].Id, WorkerId = workers[6].Id, AgreedAmount = 12_000m },

                // Local Comercial (cancelada)
                new() { WorkId = works[9].Id, WorkerId = workers[7].Id, AgreedAmount = 7_000m  },
            };
            db.WorkWorkers.AddRange(assignments);

            await db.SaveChangesAsync();

            // --------------------------------------------------------------
            // PAGOS
            // --------------------------------------------------------------
            // Se pagan algunos completos, otros parciales, otros pendientes.
            var payments = new List<Payment>();

            // Helper local: crea un pago.
            void AddPayment(int assignmentIdx, decimal amount, int daysAgo, string method, string bank, string reference, bool cancelled = false)
            {
                payments.Add(new Payment
                {
                    WorkWorkerId = assignments[assignmentIdx].Id,
                    Amount = amount,
                    Date = today.AddDays(-daysAgo),
                    PaymentMethod = method,
                    Bank = bank,
                    Reference = reference,
                    Cancelled = cancelled,
                    Notes = cancelled ? "Pago anulado por error" : null
                });
            }

            // Puente San Martín — asignaciones 0, 1, 2
            AddPayment(0, 15_000m, 60, "Transferencia", "Interbank", "OP-0001"); // pagada completa
            AddPayment(1, 6_000m, 50, "Transferencia", "BCP", "OP-0002"); // parcial
            AddPayment(1, 3_000m, 30, "Efectivo", "—", "OP-0003"); // parcial (total 9k de 12k)
            AddPayment(2, 0m, 0, "—", "—", "—", true); // cancelado (no cuenta)

            // Edificio Aurora — asignaciones 3, 4 (todo pagado)
            AddPayment(3, 45_000m, 150, "Transferencia", "Scotiabank", "OP-0100");
            AddPayment(4, 30_000m, 120, "Transferencia", "BBVA", "OP-0101");

            // Casa Los Álamos — asignaciones 5, 6
            AddPayment(5, 5_000m, 20, "Transferencia", "Interbank", "OP-0200"); // parcial
            // asignación 6: sin pagos aún (pendiente)

            // Remodelación Oficina — asignación 7 (parcial)
            AddPayment(7, 3_000m, 10, "Yape", "BCP", "OP-0300");

            // Pista Los Olivos — asignaciones 8, 9 (todo pagado)
            AddPayment(8, 22_000m, 100, "Transferencia", "Scotiabank", "OP-0400");
            AddPayment(9, 18_000m, 80, "Transferencia", "Interbank", "OP-0401");

            // Almacén Industrial — asignaciones 10, 11, 12
            AddPayment(10, 15_000m, 40, "Transferencia", "BCP", "OP-0500"); // parcial
            AddPayment(11, 10_000m, 35, "Efectivo", "—", "OP-0501"); // parcial
            // asignación 12: pendiente

            // Colegio San Marcos — asignaciones 13, 14 (todo pagado)
            AddPayment(13, 25_000m, 90, "Transferencia", "BCP", "OP-0600");
            AddPayment(14, 20_000m, 85, "Transferencia", "BBVA", "OP-0601");

            // Plaza Comercial — asignaciones 15, 16 (parciales)
            AddPayment(15, 20_000m, 25, "Transferencia", "Scotiabank", "OP-0700");
            AddPayment(16, 15_000m, 20, "Transferencia", "BCP", "OP-0701");

            // Vivienda Unifamiliar — asignación 17: pendiente (sin pagos)

            // Local Comercial — asignación 18
            AddPayment(18, 2_000m, 60, "Efectivo", "—", "OP-0900", cancelled: true); // cancelado

            db.Payments.AddRange(payments);
            await db.SaveChangesAsync();
        }
    }
}