#define CG_Debug

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace gcgcg
{
  internal class Poligono : Objeto
  {
    public Poligono(Objeto _paiRef, ref char _rotulo, List<Ponto4D> pontosPoligono) : base(_paiRef, ref _rotulo)
    {
      PrimitivaTipo = PrimitiveType.LineLoop;
      PrimitivaTamanho = 1;
      base.pontosLista = pontosPoligono;
      Atualizar();
    }

    private void Atualizar()
    {

      base.ObjetoAtualizar();
    }
    private int PtoMaisProx(Ponto4D mouse) {
      int posicao = 0;
      double ultimaDist = Distancia(pontosLista[0], mouse);
      for (int i = 0; i < pontosLista.Count; i++)
      {
        if (Distancia(pontosLista[i], mouse) < ultimaDist)
        {
          ultimaDist = Distancia(pontosLista[i], mouse);
          posicao = i;
        }
      }
      return posicao;
    }

    private double Distancia(Ponto4D pto1, Ponto4D pto2)
    {
      return Math.Sqrt(Math.Pow((pto1.X - pto2.X),2) + Math.Pow((pto1.Y - pto2.Y),2));
    }

    public void PontosAlterarMaisProximo(Ponto4D mouse)
    {
      PontosAlterar(mouse, PtoMaisProx(mouse));
    }

    public void PontosExcluirMaisProximo(Ponto4D mouse)
    {
      pontosLista.RemoveAt(PtoMaisProx(mouse));
      ObjetoAtualizar();
    }

    // public void ObjetoExcluir(Objeto objetoAtual)
    // {
      // paiRef.objetosLista.Remove(paiRef.GrafocenaBusca(objetoAtual.rotulo));
      // ObjetoAtualizar();
    // }

    public void AlterarPrimitiva() {
      if (PrimitivaTipo == PrimitiveType.LineLoop)
      {
        PrimitivaTipo = PrimitiveType.LineStrip;
      } else {
        PrimitivaTipo = PrimitiveType.LineLoop;
      }
    }

#if CG_Debug
    public override string ToString()
    {
      string retorno;
      retorno = "__ Objeto Poligono _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
      retorno += base.ImprimeToString();
      return retorno;
    }
#endif

  }
}
