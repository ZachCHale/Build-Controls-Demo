using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZHDev.CardinalDirections;

namespace Demo
{
    [CreateAssetMenu(menuName = "Demo/Building Connect Data")]

    public class BuildingConnectionData : ScriptableObject
    {
        [SerializeField] private GameObject jointReference;
        public GameObject JointReference => jointReference;
        [SerializeField] private GameObject northConnectionReference;
        public GameObject NorthConnectionReference => northConnectionReference;
        [SerializeField] private GameObject eastConnectionReference;
        public GameObject EastConnectionReference => eastConnectionReference;
        [SerializeField] private GameObject southConnectionReference;
        public GameObject SouthConnectionReference => southConnectionReference;
        [SerializeField] private GameObject westConnectionReference;
        public GameObject WestConnectionReference => westConnectionReference;

        public GameObject GetConnectionByDirection(CardinalDirection direction)
        {
            switch (direction)
            {
                case CardinalDirection.North:
                    return northConnectionReference;
                    break;
                case CardinalDirection.East:
                    return eastConnectionReference;
                    break;
                case CardinalDirection.South:
                    return southConnectionReference;
                    break;
                case CardinalDirection.West:
                    return westConnectionReference;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }
    }
}
