﻿using EvershockGame.Code.Components;
using EvershockGame.Code.Entities;
using EvershockGame.Code.Manager;
using EvershockGame.Manager;
using Microsoft.Xna.Framework;
using System;

namespace EvershockGame.Code.Factory
{
    public static class EntityFactory
    {
        public static T Create<T>(string name, Vector3? location = null) where T : class, IEntity
        {
            if (AssertManager.Get().Show(!typeof(T).IsSubclassOf(typeof(UIEntity)), "Wrong Create<T> called for UI entity. Please use CreateUI<T>."))
            {
                return CreateUI<T>(name, null);
            }
            return Create<T>(Guid.Empty, name, location);
        }

        //---------------------------------------------------------------------------

        public static T Create<T>(Guid parent, string name, Vector3? location = null) where T : class, IEntity
        {
            if (AssertManager.Get().Show(!typeof(T).IsSubclassOf(typeof(UIEntity)), "Wrong Create<T> called for UI entity. Please use CreateUI<T>."))
            {
                return CreateUI<T>(parent, name, null);
            }
            T entity = (T)Activator.CreateInstance(typeof(T), name, parent);
            return entity;
        }

        //---------------------------------------------------------------------------

        public static T CreateUI<T>(string name, Frame frame = null) where T : class, IEntity
        {
            if (AssertManager.Get().Show(typeof(T) == typeof(UIEntity) || typeof(T).IsSubclassOf(typeof(UIEntity)), "Wrong Create<T> called for entity. Please use Create<T>."))
            {
                return Create<T>(name);
            }
            return CreateUI<T>(Guid.Empty, name, frame);
        }

        //---------------------------------------------------------------------------

        public static T CreateUI<T>(Guid parent, string name, Frame frame = null) where T : class, IEntity
        {
            if (AssertManager.Get().Show(typeof(T) == typeof(UIEntity) || typeof(T).IsSubclassOf(typeof(UIEntity)), "Wrong Create<T> called for entity. Please use Create<T>."))
            {
                return Create<T>(parent, name);
            }
            T entity = (T)Activator.CreateInstance(typeof(T), name, parent, frame);
            return entity;
        }
    }
}
