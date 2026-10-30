namespace KuraVet.Domain.Services;

/// Regra de domínio que classifica o risco de um check-in a partir dos sinais vitais.
public interface ICheckinRiscoService
{
    string ClassificarRisco(int tempoPreenchimentoCapilar);
}
