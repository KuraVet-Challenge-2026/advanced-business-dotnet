using KuraVet.Domain.Common;
using KuraVet.Domain.Exceptions;

namespace KuraVet.Domain.Entities;

/// Evento da linha do tempo clínica do pet (vacina, exame, consulta, retorno, cirurgia...).
public sealed class EventoConsulta : Entidade
{
    public const int TipoTamanhoMaximo = 50;
    public const int DescricaoTamanhoMaximo = 500;
    public const int VeterinarioTamanhoMaximo = 100;

    private EventoConsulta()
    {
    }

    public EventoConsulta(int petId, string tipoEvento, DateTime dataEvento, string descricao, string veterinarioResponsavel)
    {
        Atualizar(petId, tipoEvento, dataEvento, descricao, veterinarioResponsavel);
    }

    public string TipoEvento { get; private set; } = string.Empty;

    public DateTime DataEvento { get; private set; }

    public string Descricao { get; private set; } = string.Empty;

    public string VeterinarioResponsavel { get; private set; } = string.Empty;

    public int PetId { get; private set; }

    public Pet? Pet { get; private set; }

    public void Atualizar(int petId, string tipoEvento, DateTime dataEvento, string descricao, string veterinarioResponsavel)
    {
        if (dataEvento == default)
        {
            throw new DomainException("O campo 'DataEvento' é obrigatório.");
        }

        PetId = Guard.IdentificadorValido(petId, nameof(PetId));
        TipoEvento = Guard.TextoObrigatorio(tipoEvento, nameof(TipoEvento), TipoTamanhoMaximo);
        Descricao = Guard.TextoObrigatorio(descricao, nameof(Descricao), DescricaoTamanhoMaximo);
        VeterinarioResponsavel = Guard.TextoObrigatorio(
            veterinarioResponsavel, nameof(VeterinarioResponsavel), VeterinarioTamanhoMaximo);
        DataEvento = dataEvento;
    }
}
