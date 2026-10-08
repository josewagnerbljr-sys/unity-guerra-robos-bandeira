using UnityEngine;

namespace RoboFlagWars
{
    /// <summary>Falas dos robos em nordestines bem humorado.</summary>
    public static class Dialogues
    {
        public static string Name(RobotType t)
        {
            switch (t)
            {
                case RobotType.Humanoid: return "Zé da Peixeira";
                case RobotType.Canine: return "Caramelo Arretado";
                case RobotType.Cephalopod: return "Lula Lampião";
                default: return "Camaleão Mandacaru";
            }
        }

        public static string SpecialName(RobotType t)
        {
            switch (t)
            {
                case RobotType.Humanoid: return "PEIXEIRADA";
                case RobotType.Canine: return "MIJADA";
                case RobotType.Cephalopod: return "TINTA";
                default: return "CAMUFLAR";
            }
        }

        public static string TypeDesc(RobotType t)
        {
            switch (t)
            {
                case RobotType.Humanoid: return "Bípede humanoide\nEspecial: Peixeirada (esfaqueia na frente)";
                case RobotType.Canine: return "Quadrúpede canino\nEspecial: Mijada no Poste (poça que queima e atrasa)";
                case RobotType.Cephalopod: return "Cefalópode\nEspecial: Jato de Tinta (nuvem que cega e atrasa)";
                default: return "Réptil camaleão\nEspecial: Camuflagem (some da vista)";
            }
        }

        public static string Pick(string[] a) { return a[Random.Range(0, a.Length)]; }

        public static readonly string[] Spawn = {
            "Bora, bichinho, vamo ganhar essa peleja!",
            "Oxente, cheguei! Quem é o cabra que vai me enfrentar?",
            "Eita, hoje eu tô arretado!",
            "Vixe, tô com uma fome de vitória!" };

        public static readonly string[] Kill = {
            "Toma, cabra safado!",
            "Eita lasqueira, mais um no chão!",
            "Nem sentiu, né, painho?",
            "Arriégua, acertei na mosca!",
            "Foi de bucho cheio, visse?" };

        public static readonly string[] Death = {
            "Ai, Nossa Senhora, me lascaram!",
            "Eita, bati as botas, painho!",
            "Vixe Maria, vou ali e já volto!",
            "Oxe, ainda tava comendo meu cuscuz!" };

        public static readonly string[] Stumble = {
            "Eita, buraco do cão! Cadê o chão?",
            "Oxe, quem cavou isso aqui?!",
            "Ai minha rótula, esfarelei igual cuscuz!",
            "Vixe, caí que nem jaca madura!" };

        public static readonly string[] FlagTaken = {
            "Peguei a bandeira, painho! Agora é correr!",
            "Tô com a bandeira, ninguém me pega, cabra!" };

        public static readonly string[] FlagCaptured = {
            "Bandeira no bolso, bora pro forró!",
            "Arretado! Ponto pra nós, visse!" };

        public static readonly string[] FlagLost = {
            "Eita, derrubei a bandeira, vixe!",
            "Ave Maria, lá se foi a bandeira!" };

        public static string[] Special(RobotType t)
        {
            switch (t)
            {
                case RobotType.Humanoid:
                    return new[] { "Toma peixeirada, cabra!", "Vou te fazer um corte de cuscuz!", "Peixeira de Pernambuco, meu fi!" };
                case RobotType.Canine:
                    return new[] { "Marquei o poste, é meu território, visse!", "Mijei, ô! Num entra aqui não!", "Au-au, oxente, esse poste é meu!" };
                case RobotType.Cephalopod:
                    return new[] { "Toma tinta, cabra, vai ficar no escuro!", "Jato de tinta, painho! Ninguém me vê!", "Vixe, virei lula na chapa!" };
                default:
                    return new[] { "Sumi igual dinheiro no fim do mês!", "Num me enxerga mais, né, oxente?", "Camuflei, painho, nem a sogra me acha!" };
            }
        }
    }
}
