﻿using EvershockGame.Code;
using EvershockGame.Code.Factories;
using EvershockGame.Code.Items;
using EvershockGame.Code.Manager;
using EvershockGame.Manager;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvershockGame.Code.Components
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
            UpdateWeapon();
        }

        //---------------------------------------------------------------------------

        public void SelectNext()
        {
            ActiveIndex = (ActiveIndex + 1) % Size;
            UpdateWeapon();
        }

        //---------------------------------------------------------------------------

        public int TryAdd(EItemType type, int count)
        {
            ItemDesc item = ItemManager.Get().Find(type);
            if (!item.IsEmpty)
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
                            if (index == ActiveIndex) if (index == ActiveIndex) UpdateWeapon();
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

        public void TryDrop(EItemType type)
        {
            int index = GetFirstSlot(type);
            if (index >= 0)
            {
                InventorySlot slot = m_Items[index];
                if (slot != null && slot.Drop())
                {
                    TransformComponent transform = GetComponent<TransformComponent>();
                    if (transform != null)
                    {
                        PickupFactory.Create(slot.Item.Type, transform.Location, new Vector3(transform.Orientation.X * 10, transform.Orientation.Y * 10, 40));
                    }
                    if (slot.Count == 0)
                    {
                        m_Items[index] = null;
                    }
                }
            }
        }

        //---------------------------------------------------------------------------

        public void TryDrop(int index, int count)
        {
            InventorySlot slot = m_Items[index];
            if (slot != null && slot.Drop())
            {
                TransformComponent transform = GetComponent<TransformComponent>();

                if (transform != null)
                {
                    PickupFactory.Create(slot.Item.Type, transform.Location, new Vector3(transform.Orientation.X * 600, transform.Orientation.Y * 600, 40));
                }

                if (slot.Count == 0)
                {
                    m_Items[index] = null;
                }
            }
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
                    if (slot.Use())
                    {
                        if (slot.Count == 0)
                        {
                            m_Items[index] = null;
                        }
                    }
                }
            }
        }

        //---------------------------------------------------------------------------

        public void TryUse(int index)
        {
            InventorySlot slot = m_Items[index];
            if (slot != null && slot.Use())
            {
                foreach (Weapon weapon in GetChildren<Weapon>())
                {
                    weapon.TryAttack();
                }

                if (slot.Count == 0)
                {
                    m_Items[index] = null;
                }
            }
        }

        //---------------------------------------------------------------------------

        private void UpdateWeapon()
        {
            InventorySlot slot = ActiveSlot;
            List<Weapon> weapons = GetChildren<Weapon>();
            if (weapons != null && weapons.Count > 0)
            {
                weapons.First().TryUpdate(slot != null ? slot.Item : ItemDesc.Empty);
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
            else if (actions[EGameAction.INVENTORY_USE_ITEM].IsPressed)
            {
                TryUse(ActiveIndex);
            }
            else if (actions[EGameAction.INVENTORY_DROP_ITEM].IsPressed)
            {
                TryDrop(ActiveIndex, 1);
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

        public bool Drop()
        {
            if (Count > 0)
            {
                Count--;
                return true;
            }
            else
            {
                return false;
            }
        }

        //---------------------------------------------------------------------------

        public bool Use()
        {
            if (Item.IsConsumable)
            {
                if (Count > 0)
                {
                    Count--;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}
