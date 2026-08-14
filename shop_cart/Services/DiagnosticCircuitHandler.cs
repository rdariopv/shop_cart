using Microsoft.AspNetCore.Components.Server.Circuits;

namespace shop_cart.Services
{
    
        public class DiagnosticCircuitHandler : CircuitHandler
        {
            private readonly ILogger<DiagnosticCircuitHandler> _logger;

            public DiagnosticCircuitHandler(ILogger<DiagnosticCircuitHandler> logger)
            {
                _logger = logger;
            }

            public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken ct)
            {
                _logger.LogInformation("[Circuit] Abierto: {CircuitId}", circuit.Id);
                return base.OnCircuitOpenedAsync(circuit, ct);
            }

            public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken ct)
            {
                _logger.LogInformation("[Circuit] Cerrado: {CircuitId}", circuit.Id);
                return base.OnCircuitClosedAsync(circuit, ct);
            }

            public override Task OnConnectionDownAsync(Circuit circuit, CancellationToken ct)
            {
                _logger.LogError("[Circuit] Conexión caída: {CircuitId}", circuit.Id);
                return base.OnConnectionDownAsync(circuit, ct);
            }
        }
    
}
