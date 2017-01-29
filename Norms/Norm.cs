using System;
using System.Collections.Generic;
using Assets.Scripts.Ant;


namespace Assets.Scripts.Norms
{
    [Serializable]
    public class Norm
    {
        public bool IsActive;
        public string Name = "";
        public DeonticOperator DeonticOperator = DeonticOperator.Obligation;
        public Role Target = Role.Worker;
        public Role Evaluator = Role.Submitter;
        public Tag PertinenceCondition = Tag.OnContractFinish;
        public Action Content = Action.Accept;
        public List<Policy> Policies = new List<Policy>();

        public Norm()
        {

        }
    }

    public enum Role
    {
        Worker, Submitter
    }

    public enum DeonticOperator
    {
        Obligation, Interdiction
    }
    
    public enum Tag
    {
        OnContractStart, OnContractFinish
    }

    public enum Action
    {
        Accept, Deny
    }
}
