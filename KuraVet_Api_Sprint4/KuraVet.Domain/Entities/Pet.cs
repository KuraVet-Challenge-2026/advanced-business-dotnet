using KuraVet.Domain.Common;

namespace KuraVet.Domain.Entities;

public sealed class Pet : Entidade
{
    public const int NomeTamanhoMaximo = 100;

    private Pet()
    {
    }

    public Pet(string nome, int tutorId)
    {
        Atualizar(nome, tutorId);
    }

    public string Nome { get; private set; } = string.Empty;

    public int TutorId { get; private set; }

    public Tutor? Tutor { get; private set; }

    public ICollection<CheckinHistorico> Checkins { get; private set; } = new List<CheckinHistorico>();

    public void Atualizar(string nome, int tutorId)
    {
        Nome = Guard.TextoObrigatorio(nome, nameof(Nome), NomeTamanhoMaximo);
        TutorId = Guard.IdentificadorValido(tutorId, nameof(TutorId));
    }
}
