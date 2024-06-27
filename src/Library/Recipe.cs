//-------------------------------------------------------------------------
// <copyright file="Recipe.cs" company="Universidad Católica del Uruguay">
// Copyright (c) Programación II. Derechos reservados.
// </copyright>
//-------------------------------------------------------------------------
// Patrón(es) o principio(s) usados:
// - Single Responsibility Principle (SRP): La clase Recipe maneja los pasos de una receta y el estado de cocción.
// - Open/Closed Principle (OCP): La clase Recipe puede ser extendida sin modificar su comportamiento existente.
// - Dependency Inversion Principle (DIP): La clase Recipe depende de abstracciones (IRecipeContent y TimerClient).
// - Interface Segregation Principle (ISP): Se define la interfaz IRecipeContent para asegurar que las clases solo implementen métodos necesarios.

using System;
using System.Collections.Generic;
using System.Threading;

namespace Full_GRASP_And_SOLID
{
    public class Recipe : IRecipeContent, TimerClient // Modificado por DIP y para implementar TimerClient
    {
        private IList<BaseStep> steps = new List<BaseStep>();
        private bool cooked = false;
        private CountdownTimer timer;

        public Product FinalProduct { get; set; }

        // Agregado por Creator
        public void AddStep(Product input, double quantity, Equipment equipment, int time)
        {
            Step step = new Step(input, quantity, equipment, time);
            this.steps.Add(step);
        }

        // Agregado por OCP y Creator
        public void AddStep(string description, int time)
        {
            WaitStep step = new WaitStep(description, time);
            this.steps.Add(step);
        }

        public void RemoveStep(BaseStep step)
        {
            this.steps.Remove(step);
        }

        // Agregado por SRP
        public string GetTextToPrint()
        {
            string result = $"Receta de {this.FinalProduct.Description}:\n";
            foreach (BaseStep step in this.steps)
            {
                result = result + step.GetTextToPrint() + "\n";
            }

            // Agregado por Expert
            result = result + $"Costo de producción: {this.GetProductionCost()}";

            return result;
        }

        // Agregado por Expert
        public double GetProductionCost()
        {
            double result = 0;

            foreach (BaseStep step in this.steps)
            {
                result = result + step.GetStepCost();
            }

            return result;
        }

        // Método para obtener el tiempo total de cocción
        public int GetCookTime()
        {
            int totalCookTime = 0;
            foreach (BaseStep step in this.steps)
            {
                totalCookTime += step.Time;
            }
            return totalCookTime;
        }

        // Propiedad de solo lectura que indica si la receta está cocida
        public bool Cooked
        {
            get { return cooked; }
        }

        // Método para cocinar la receta
        public void Cook()
        {
            timer = new CountdownTimer();
            timer.Register(GetCookTime(), this);
        }

        // Implementación de la interfaz TimerClient
        public void TimeOut()
        {
            cooked = true;
        }
    }
}
