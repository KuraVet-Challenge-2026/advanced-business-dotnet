using KuraVet.Domain.Entities;

namespace KuraVet.Domain.Services;

/// TPC acima de 2 segundos indica risco moderado.
public sealed class CheckinRiscoService : ICheckinRiscoService
{
    private const int LimiteTpcSegundos = 2;

    public string ClassificarRisco(int tempoPreenchimentoCapilar)
    {
        return tempoPreenchimentoCapilar > LimiteTpcSegundos ? NiveisRisco.Moderado : NiveisRisco.Baixo;
    }
}
