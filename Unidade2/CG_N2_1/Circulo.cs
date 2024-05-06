#define CG_Debug

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
    internal class Circulo : Objeto
    {
        public Circulo(Objeto paiRef, ref char _rotulo, float radius) : base(paiRef, ref _rotulo)
        {
            PrimitivaTipo = PrimitiveType.Points;
            PrimitivaTamanho = 5;

            int angle = 360/72;

            for (int i = 0; i < 360; i += angle) {
                Ponto4D pto = Matematica.GerarPtosCirculo(i, radius);
                base.PontosAdicionar(pto);
            }
            
            Atualizar();
        }

        public void Atualizar()
        {
            base.ObjetoAtualizar();
        }
    }
}