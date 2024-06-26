﻿using System;
using System.Collections.Generic;
using System.Linq;
using FrameworkSDK.Localization;
using JetBrains.Annotations;

namespace FrameworkSDK.Pipelines
{
    public class PipelineStep : INamed
    {
        public string Name { get; }

        public PipelineStep([NotNull] string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        [NotNull, ItemNotNull] public IReadOnlyList<IPipelineAction> Actions => _actions;

        private readonly List<IPipelineAction> _actions = new List<IPipelineAction>();

        [NotNull]
        public IPipelineAction this[[NotNull] string actionName]
        {
            get
            {
                if (actionName == null) throw new ArgumentNullException(nameof(actionName));
                if (string.IsNullOrWhiteSpace(actionName)) throw new ArgumentException(nameof(actionName));

                if (!ContainsWithName(actionName))
                    throw new PipelineException(Strings.Exceptions.Pipeline.ActionNotFound, actionName);

                return _actions.First(action => action.Name == actionName);
            }
        }

	    public override string ToString()
	    {
		    return Name;
	    }

	    public void AddAction([NotNull] IPipelineAction action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (ContainsWithName(action.Name))
                throw new PipelineException(Strings.Exceptions.Pipeline.ActionAlreadyExists, action.Name);

            _actions.Add(action);
        }

        public void RemoveAction([NotNull] IPipelineAction action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (!ContainsWithName(action.Name))
                throw new PipelineException(Strings.Exceptions.Pipeline.ActionNotFound, action.Name);

            _actions.Remove(action);
        }

        public bool ContainsAction([NotNull] IPipelineAction action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            return _actions.Contains(action);
        }

        public virtual IReadOnlyList<IPipelineAction> GetActionsForProcess()
        {
            return Actions.ToArray();
        }

        private bool ContainsWithName(string actionName)
        {
            return _actions.ContainsByName(actionName);
        }
    }
}