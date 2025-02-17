using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace DialogueSystem
{
    public class KitchenHandler : MonoBehaviour
    {
        public KitchenType currentKitchenType = KitchenType.none;
        public List<Transform> kitchenPoints;
        public List<bool> occupiedPlace = new List<bool>();

        private void Start()
        {
            for(var i = 0; i < kitchenPoints.Count; i++) occupiedPlace.Add(false);
        }

        public kitchenPlace getCurrentKitchenPoint()
        {
            return currentKitchenType switch
            {
                KitchenType.none => new kitchenPlace(Vector2.zero, KitchenType.none, false),
                KitchenType.stove => new kitchenPlace(kitchenPoints[0].position, KitchenType.stove, occupiedPlace[0]),
                KitchenType.oven => new kitchenPlace(kitchenPoints[1].position, KitchenType.oven, occupiedPlace[1]),
                KitchenType.cutter => new kitchenPlace(kitchenPoints[2].position, KitchenType.cutter, occupiedPlace[2]),
                KitchenType.mixer => new kitchenPlace(kitchenPoints[3].position, KitchenType.mixer, occupiedPlace[3]),
                KitchenType.plate => new kitchenPlace(kitchenPoints[4].position, KitchenType.plate, occupiedPlace[4]),
                KitchenType.trachcan => new kitchenPlace(new Vector2(-1, -1), KitchenType.trachcan, false),
                _ => new kitchenPlace(Vector2.zero, KitchenType.none, false)
            };
        }

        public void setKitchenOccupied(KitchenType kitchenType, bool occupied)
        {
            switch (kitchenType)
            {
                case KitchenType.stove:
                    occupiedPlace[0] = occupied;
                    break;
                case KitchenType.oven:
                    occupiedPlace[1] = occupied;
                    break;
                case KitchenType.cutter:
                    occupiedPlace[2] = occupied;
                    break;
                case KitchenType.mixer:
                    occupiedPlace[3] = occupied;
                    break;
                case KitchenType.plate:
                    occupiedPlace[4] = occupied;
                    break;
            }
        }
        
    }

    public class kitchenPlace
    {
        public Vector2 position;

        public KitchenType kitchenType;
        
        public bool occupied;

        public kitchenPlace(Vector2 pos, KitchenType kitchenType, bool occupied)
        {
            position = pos;
            this.kitchenType = kitchenType;
            this.occupied = occupied;
        }
    }
    
    public enum KitchenType
    {
        none,
        stove,
        oven,
        cutter,
        mixer,
        plate,
        trachcan
    }
}