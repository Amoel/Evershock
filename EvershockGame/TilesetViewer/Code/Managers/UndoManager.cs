using Level;
using Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilesetViewer
{
    public class UndoManager : BaseManager<UndoManager>
    {
        private Stack<UndoAction> m_UndoActions;
        private Stack<UndoAction> m_RedoActions;

        private UndoAction m_CurrentUndo;

        //---------------------------------------------------------------------------

        protected UndoManager()
        {
            m_UndoActions = new Stack<UndoAction>();
            m_RedoActions = new Stack<UndoAction>();
        }

        //---------------------------------------------------------------------------

        public void StartUndo(ELayerMode mode)
        {
            m_CurrentUndo = new UndoAction(mode);
        }

        //---------------------------------------------------------------------------

        public void AddUndoState(UndoState before, UndoState after)
        {
            m_CurrentUndo?.Add(before, after);
        }

        //---------------------------------------------------------------------------

        public void StopUndo()
        {
            if (m_CurrentUndo != null && m_CurrentUndo.ContainsUndos)
            {
                m_RedoActions.Clear();
                m_UndoActions.Push(m_CurrentUndo);
            }
            m_CurrentUndo = null;
        }

        //---------------------------------------------------------------------------

        public void Undo()
        {
            if (m_UndoActions.Count > 0)
            {
                UndoAction action = m_UndoActions.Pop();
                action.ExecuteUndo();
                m_RedoActions.Push(action);
            }
        }

        //---------------------------------------------------------------------------

        public void Redo()
        {
            if (m_RedoActions.Count > 0)
            {
                UndoAction action = m_RedoActions.Pop();
                action.ExecuteRedo();
                m_UndoActions.Push(action);
            }
        }
    }

    //---------------------------------------------------------------------------

    public class UndoAction
    {
        public ELayerMode Layer { get; private set; }
        public List<UndoState> Before { get; private set; }
        public List<UndoState> After { get; private set; }

        public bool ContainsUndos { get { return Before.Count > 0 && After.Count > 0; } }

        //---------------------------------------------------------------------------

        public UndoAction(ELayerMode layer)
        {
            Layer = layer;
            Before = new List<UndoState>();
            After = new List<UndoState>();
        }

        //---------------------------------------------------------------------------

        public void Add(UndoState before, UndoState after)
        {
            Before.Add(before);
            After.Add(after);
        }

        //---------------------------------------------------------------------------

        public void ExecuteUndo()
        {
            foreach (UndoState state in Before)
            {
                LevelManager.Get().SetTile(Layer, state.SourceX, state.SourceY, state.DestinationX, state.DestinationY, state.IsBlocked);
            }
        }

        //---------------------------------------------------------------------------

        public void ExecuteRedo()
        {
            foreach (UndoState state in After)
            {
                LevelManager.Get().SetTile(Layer, state.SourceX, state.SourceY, state.DestinationX, state.DestinationY, state.IsBlocked);
            }
        }
    }

    //---------------------------------------------------------------------------

    public class UndoState
    {
        public int SourceX { get; private set; }
        public int SourceY { get; private set; }
        public int DestinationX { get; private set; }
        public int DestinationY { get; private set; }
        public bool IsBlocked { get; private set; }
        public int Value { get; private set; }

        //---------------------------------------------------------------------------

        public UndoState(int sourceX, int sourceY, int destinationX, int destinationY, bool isBlocked)
        {
            SourceX = sourceX;
            SourceY = sourceY;
            DestinationX = destinationX;
            DestinationY = destinationY;
            IsBlocked = isBlocked;
        }
    }
}
