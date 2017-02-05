using EntityComponent.Items;
using EntityComponent.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Components
{
    public class InventoryComponent : Component, IInputReceiver
    {
        public int Size { get { return m_Items.Length; } }

        public int ActiveIndex { get; private set; }
        public InventorySlot ActiveSlot { get { return m_Items[ActiveIndex]; } }

        private InventorySlot[] m_Items;

        //---------------------------------------------------------------------------

        public InventoryComponent(Guid entity) : base(entity) { m_Items = new InventorySlot[5]; }

        //---------------------------------------------------------------------------

        public InventorySlot this[int index]
        {
            get { return m_Items[index]; }
        }

        //---------------------------------------------------------------------------

        public void SelectPrevious()
        {
            ActiveIndex = (ActiveIndex + Size - 1) % Size;
        }

        //---------------------------------------------------------------------------

        public void SelectNext()
        {
            ActiveIndex = (ActiveIndex + 1) % Size;
        }

        //---------------------------------------------------------------------------

        public int TryAdd(EItemType type, int count)
        {
            ItemDesc item = ItemManager.Get().Find(type);
            if (item.IsValid)
            {
                int remaining = count;
                int index = 0;
                do
                {
                    index = GetFirstNonFullSlot(type);
                    if (index >= 0)
                    {
                        if (m_Items[index] == null)
                        {
                            m_Items[index] = new InventorySlot(item);
                        }
                        remaining = m_Items[index].Add(remaining);
                    }
                }
                while (remaining > 0 && index >= 0);
                return remaining;
            }
            return count;
        }

        //---------------------------------------------------------------------------

        public void TryDrop(EItemType type, int count)
        {
            int remaining = count;
            int index = 0;
            do
            {
                index = GetFirstSlot(type);
                if (index >= 0)
                {
                    InventorySlot slot = m_Items[index];
                    if (slot != null)
                    {
                        remaining = slot.Drop(remaining);
                        if (slot.Count == 0)
                        {
                            m_Items[index] = null;
                        }
                    }
                }
            }
            while (remaining > 0 && index >= 0);
        }

        //---------------------------------------------------------------------------

        public void TryUse(EItemType type)
        {
            int index = GetFirstSlot(type);
            if (index >= 0)
            {
                InventorySlot slot = m_Items[index];
                if (slot != null)
                {
                    slot.Use();
                    if (slot.Count == 0)
                    {
                        m_Items[index] = null;
                    }
                }
            }
        }

        //---------------------------------------------------------------------------

        private int GetEmptySlot()
        {
            return Array.IndexOf(m_Items, null);
        }

        //---------------------------------------------------------------------------

        private int GetFirstSlot(EItemType type)
        {
            return Array.FindIndex(m_Items, item => item.Item.Type == type);
        }

        //---------------------------------------------------------------------------

        private int GetFirstNonFullSlot(EItemType type)
        {
            return Array.FindIndex(m_Items, slot => slot == null || (slot.Item.Type == type && slot.Count < slot.MaxCount));
        }

        //---------------------------------------------------------------------------

        public void ReceiveInput(GameActionCollection actions, float deltaTime)
        {
            if (actions[EGameAction.INVENTORY_PREVIOUS_ITEM].IsPressed)
            {
                SelectPrevious();
            }
            else if (actions[EGameAction.INVENTORY_NEXT_ITEM].IsPressed)
            {
                SelectNext();
            }
        }

        //---------------------------------------------------------------------------

        public override void OnCleanup() { }
    }

    //---------------------------------------------------------------------------

    public class InventorySlot
    {
        public ItemDesc Item { get; private set; }
        public int Count { get; private set; }

        public int MaxCount { get { return Item.IsStackable ? ItemDesc.MaxStack : 1; } }

        //---------------------------------------------------------------------------

        public InventorySlot(ItemDesc item)
        {
            Item = item;
        }

        //---------------------------------------------------------------------------

        public int Add(int count)
        {
            int temp = Math.Min(MaxCount - Count, count);
            Count += temp;
            return count - temp;
        }

        //---------------------------------------------------------------------------

        public int Drop(int count)
        {
            int temp = Math.Min(Count, count);
            Count -= temp;
            return count - temp;
        }

        //---------------------------------------------------------------------------

        public void Use()
        {
            Count--;
        }
    }
}
