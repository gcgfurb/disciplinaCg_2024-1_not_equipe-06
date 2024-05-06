#define CG_Debug

using System;
using System.Collections.Generic;
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
    internal class Spline : Objeto
    {
        private List<Ponto> controlPoints;
        private List<SegReta> edgeLines;
        
        private Shader defaultSplineColor;
        private Shader selectedPointColor;
        private Shader controlPointsColor;
        private Shader controlSplineColor;

        private int selectedPoint;

        private double bezierCoefficient;
        private int bezierPoints;
        public int BezierPoints { get => bezierPoints; set => bezierPoints = value; }
        private List<Ponto4D> bezierPointsList;
        private List<SegReta> bezierLinesList;

        public Spline(Objeto paiRef, ref char _rotulo) : base(paiRef, ref _rotulo)
        {
            PrimitivaTipo = PrimitiveType.Lines;
            PrimitivaTamanho = 10;

            controlPoints = new List<Ponto>();
            edgeLines = new List<SegReta>();
            bezierPointsList = new List<Ponto4D>();
            bezierLinesList = new List<SegReta>();

            controlSplineColor = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
            controlPointsColor = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
            defaultSplineColor = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");
            selectedPointColor = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");

            bezierPoints = 10;
            
            Atualizar();
        }

        private void AddPoints(char _rotulo = '@') 
        {
            controlPoints.Add(new Ponto(this, ref _rotulo, new Ponto4D(x: 0.5, y: -0.5)));
            controlPoints.Add(new Ponto(this, ref _rotulo, new Ponto4D(x: -0.5, y: -0.5)));
            controlPoints.Add(new Ponto(this, ref _rotulo, new Ponto4D(x: -0.5, y: 0.5)));
            controlPoints.Add(new Ponto(this, ref _rotulo, new Ponto4D(x: 0.5, y: 0.5)));

            foreach (Ponto point in controlPoints) {
                point.PrimitivaTamanho = 20;
                point.ShaderObjeto = controlPointsColor;
            }

            controlPoints[selectedPoint].ShaderObjeto = selectedPointColor;
        }

        private void AddEdgeLines(char _rotulo = '@') 
        {
            edgeLines.Add(new SegReta(this, ref _rotulo, controlPoints[0].PontosId(0), controlPoints[3].PontosId(0)));
            edgeLines.Add(new SegReta(this, ref _rotulo, controlPoints[1].PontosId(0), controlPoints[2].PontosId(0)));
            edgeLines.Add(new SegReta(this, ref _rotulo, controlPoints[2].PontosId(0), controlPoints[3].PontosId(0)));
           
            foreach (SegReta line in edgeLines) {
                line.ShaderObjeto = defaultSplineColor;
            }
        }

        private void AddBezierCurve() {
            bezierCoefficient = GetBezierCoefficient(bezierPoints);
            CalculateBezierPoints();
            CreateBezierLines();
        }

        private void CalculateBezierPoints()
        {
            for (double t = 0.0; t <= 1; t += bezierCoefficient)
            {
                bezierPointsList.Add(CalculatePoint(t));
            }

            bezierPointsList.Add(CalculatePoint(1.0));
        }

        private void CreateBezierLines(char _rotulo = '@')
        {
            for (int i = 0; i < bezierPointsList.Count - 1; i++)
            {
                SegReta line = new SegReta(this, ref _rotulo, bezierPointsList[i], bezierPointsList[i + 1])
                {
                    ShaderObjeto = controlSplineColor
                };
                bezierLinesList.Add(line);
            }
        }

        private void RemoveBezierPoints() {
            foreach (SegReta line in bezierLinesList) 
            {
                FilhoRemover(line);
            }

            bezierPointsList.Clear();
            bezierLinesList.Clear();
        }

        private Ponto4D CalculatePoint(double t) {
            double u = 1 - t;
            double t2 = t * t;
            double u2 = u * u;
            double u3 = u2 * u;
            double t3 = t2 * t;

            double x = u3 * controlPoints[1].PontosId(0).X + 
                3 * t * u2 * controlPoints[2].PontosId(0).X + 
                3 * t2 * u * controlPoints[3].PontosId(0).X + 
                t3 * controlPoints[0].PontosId(0).X;

            double y = u3 * controlPoints[1].PontosId(0).Y + 
                3 * t * u2 * controlPoints[2].PontosId(0).Y + 
                3 * t2 * u * controlPoints[3].PontosId(0).Y + 
                t3 * controlPoints[0].PontosId(0).Y;

            return new Ponto4D(x, y);
        }

        private double GetBezierCoefficient(int points) {
            return 1.0 / points;
        } 

        public void Atualizar()
        {
            AddPoints();
            AddEdgeLines();
            AddBezierCurve();

            base.ObjetoAtualizar();
        }

        public void ChangeSelectedPoint() {
            int index = selectedPoint;
            
            if (selectedPoint < 3) {
                selectedPoint++;
            } else {
                selectedPoint = 0;
            }

            controlPoints[index].ShaderObjeto = controlPointsColor;
            controlPoints[selectedPoint].ShaderObjeto = selectedPointColor;
        }

        public void MoveSelectedPoint(char direction) {
            Ponto4D nextLocation = new Ponto4D();
            Ponto currentLocation = controlPoints[selectedPoint];
            double movement = 0.05;

            switch (direction) 
            {
                case 'C': 
                {
                    nextLocation = new Ponto4D(x: currentLocation.PontosId(0).X, y: currentLocation.PontosId(0).Y + movement);
                    break;
                }
                case 'B': 
                {
                    nextLocation = new Ponto4D(x: currentLocation.PontosId(0).X, y: currentLocation.PontosId(0).Y - movement);
                    break;
                }
                case 'E': 
                {
                    nextLocation = new Ponto4D(x: currentLocation.PontosId(0).X - movement, y: currentLocation.PontosId(0).Y);
                    break;
                }
                case 'D': 
                {
                    nextLocation = new Ponto4D(x: currentLocation.PontosId(0).X + movement, y: currentLocation.PontosId(0).Y);
                    break;
                }
            }

            switch (selectedPoint) 
            {
                case 0: 
                {
                    edgeLines[0].PontosAlterar(nextLocation, 0);
                    edgeLines[0].ObjetoAtualizar();
                    break;
                }
                case 1:
                {
                    edgeLines[1].PontosAlterar(nextLocation, 0);
                    edgeLines[1].ObjetoAtualizar();
                    break;
                }
                case 2:
                {
                    edgeLines[1].PontosAlterar(nextLocation, 1);
                    edgeLines[2].PontosAlterar(nextLocation, 0);
                    edgeLines[1].ObjetoAtualizar();
                    edgeLines[2].ObjetoAtualizar();
                    break;
                }
                case 3:
                {
                    edgeLines[0].PontosAlterar(nextLocation, 1);
                    edgeLines[2].PontosAlterar(nextLocation, 1);
                    edgeLines[0].ObjetoAtualizar();
                    edgeLines[2].ObjetoAtualizar();
                    break;
                }
            }

            currentLocation.PontosAlterar(nextLocation, 0);
            currentLocation.ObjetoAtualizar();
            
            ChangeSplinePoint();
        }

        public void ChangeSplinePoint() 
        {
            RemoveBezierPoints();
            AddBezierCurve();
        }
    }
}
