namespace KuraVet.Api.Domain
{
    public class CheckinRiscoService : ICheckinRiscoService
    {
        private const int LimiteTpcSegundos = 2;

        public string ClassificarRisco(int tempoPreenchimentoCapilar)
        {
            return tempoPreenchimentoCapilar > LimiteTpcSegundos ? "Moderado" : "Baixo";
        }
    }
}
