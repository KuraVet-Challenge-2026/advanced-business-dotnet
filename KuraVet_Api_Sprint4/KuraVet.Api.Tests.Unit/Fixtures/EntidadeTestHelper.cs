using KuraVet.Domain.Common;

namespace KuraVet.Api.Tests.Unit.Fixtures;

/// Simula o Id gerado pelo banco
public static class EntidadeTestHelper
{
    public static T ComId<T>(this T entidade, int id) where T : Entidade
    {
        typeof(Entidade).GetProperty(nameof(Entidade.Id))!.SetValue(entidade, id);
        return entidade;
    }
}
