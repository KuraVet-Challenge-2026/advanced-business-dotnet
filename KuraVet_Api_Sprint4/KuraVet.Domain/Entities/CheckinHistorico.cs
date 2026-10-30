using KuraVet.Domain.Common;

namespace KuraVet.Domain.Entities;

/// Check-in de sinais vitais registrado pelo tutor (frequência respiratória, TPC, mucosa e hidratação).
public sealed class CheckinHistorico : Entidade
{
    public const int TextoTamanhoMaximo = 50;
    public const int FrequenciaRespiratoriaMinima = 1;
    public const int FrequenciaRespiratoriaMaxima = 300;
    public const int TpcMinimo = 0;
    public const int TpcMaximo = 30;

    private CheckinHistorico()
    {
    }

    public CheckinHistorico(
        int petId,
        int frequenciaRespiratoria,
        int tempoPreenchimentoCapilar,
        string corMucosa,
        string nivelHidratacao,
        DateTime? dataCheckin = null)
    {
        AtualizarSinais(petId, frequenciaRespiratoria, tempoPreenchimentoCapilar, corMucosa, nivelHidratacao, dataCheckin);
    }

    public DateTime DataCheckin { get; private set; }

    public int FrequenciaRespiratoria { get; private set; }

    public int TempoPreenchimentoCapilar { get; private set; }

    public string CorMucosa { get; private set; } = string.Empty;

    public string NivelHidratacao { get; private set; } = string.Empty;

    public string? NivelRiscoIA { get; private set; }

    public int PetId { get; private set; }

    public Pet? Pet { get; private set; }

    public void AtualizarSinais(
        int petId,
        int frequenciaRespiratoria,
        int tempoPreenchimentoCapilar,
        string corMucosa,
        string nivelHidratacao,
        DateTime? dataCheckin = null)
    {
        PetId = Guard.IdentificadorValido(petId, nameof(PetId));
        FrequenciaRespiratoria = Guard.Intervalo(
            frequenciaRespiratoria, FrequenciaRespiratoriaMinima, FrequenciaRespiratoriaMaxima, nameof(FrequenciaRespiratoria));
        TempoPreenchimentoCapilar = Guard.Intervalo(
            tempoPreenchimentoCapilar, TpcMinimo, TpcMaximo, nameof(TempoPreenchimentoCapilar));
        CorMucosa = Guard.TextoObrigatorio(corMucosa, nameof(CorMucosa), TextoTamanhoMaximo);
        NivelHidratacao = Guard.TextoObrigatorio(nivelHidratacao, nameof(NivelHidratacao), TextoTamanhoMaximo);
        DataCheckin = dataCheckin ?? (DataCheckin == default ? DateTime.UtcNow : DataCheckin);
    }

    public void DefinirNivelRisco(string nivelRisco)
    {
        NivelRiscoIA = NiveisRisco.Normalizar(nivelRisco);
    }
}
