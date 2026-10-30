using KuraVet.Domain.Common;

namespace KuraVet.Domain.Entities;

public sealed class Tutor : Entidade
{
    public const int NomeTamanhoMaximo = 100;

    private Tutor()
    {
    }

    public Tutor(string nome)
    {
        AlterarNome(nome);
    }

    public string Nome { get; private set; } = string.Empty;

    public ICollection<Pet> Pets { get; private set; } = new List<Pet>();

    public void AlterarNome(string nome)
    {
        Nome = Guard.TextoObrigatorio(nome, nameof(Nome), NomeTamanhoMaximo);
    }
}
