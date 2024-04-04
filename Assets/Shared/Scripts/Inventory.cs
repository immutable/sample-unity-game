using System;
using System.Collections;
using System.Collections.Generic;
using HyperCasual.Core;
using HyperCasual.Gameplay;
using UnityEngine;

namespace HyperCasual.Runner
{
    /// <summary>
    /// A simple inventory class that listens to game events and keeps track of the amount of in-game currencies
    /// collected by the player
    /// </summary>
    public class Inventory : AbstractSingleton<Inventory>
    {
        [SerializeField]
        GenericGameEventListener m_FoodEventListener;
        [SerializeField]
        GenericGameEventListener m_CoinEventListener;
        [SerializeField]
        GenericGameEventListener m_WinEventListener;
        [SerializeField]
        GenericGameEventListener m_LoseEventListener;

        int m_TempFood;
        int m_TotalFood;
        float m_TempXp;
        float m_TotalXp;
        int m_TempCoins;

        /// <summary>
        /// Temporary const
        /// Users keep accumulating XP when playing the game and they're rewarded as they hit a milestone.
        /// Milestones are simply a threshold to reward users for playing the game. We need to come up with
        /// a proper formula to calculate milestone values but because we don't have a plan for the milestone
        /// rewards yet, we have simple set the value to something users can never reach. 
        /// </summary>
        const float k_MilestoneFactor = 1.2f;

        Hud m_Hud;
        LevelCompleteScreen m_LevelCompleteScreen;

        void Start()
        {
            m_FoodEventListener.EventHandler = OnFoodPicked;
            m_CoinEventListener.EventHandler = OnCoinPicked;
            m_WinEventListener.EventHandler = OnWin;
            m_LoseEventListener.EventHandler = OnLose;

            m_TempFood = 0;
            m_TotalFood = SaveManager.Instance.Food;
            m_TempXp = 0;
            m_TotalXp = SaveManager.Instance.XP;
            m_TempCoins = 0;

            m_LevelCompleteScreen = UIManager.Instance.GetView<LevelCompleteScreen>();
            m_Hud = UIManager.Instance.GetView<Hud>();
        }

        void OnEnable()
        {
            m_FoodEventListener.Subscribe();
            m_CoinEventListener.Subscribe();
            m_WinEventListener.Subscribe();
            m_LoseEventListener.Subscribe();
        }

        void OnDisable()
        {
            m_FoodEventListener.Unsubscribe();
            m_CoinEventListener.Unsubscribe();
            m_WinEventListener.Unsubscribe();
            m_LoseEventListener.Unsubscribe();
        }

        void OnFoodPicked()
        {
            if (m_FoodEventListener.m_Event is ItemPickedEvent foodPickedEvent)
            {
                m_TempFood += foodPickedEvent.Count;
                m_Hud.FoodValue = m_TempFood;
            }
            else
            {
                throw new Exception($"Invalid event type!");
            }
        }

        void OnCoinPicked()
        {
            if (m_CoinEventListener.m_Event is ItemPickedEvent coinPickedEvent)
            {
                m_TempCoins += coinPickedEvent.Count;
            }
            else
            {
                throw new Exception($"Invalid event type!");
            }
        }

        void OnWin()
        {
            m_TotalFood += m_TempFood;
            m_TempFood = 0;
            SaveManager.Instance.Food = m_TotalFood;

            m_LevelCompleteScreen.FoodValue = m_TotalFood;
            m_LevelCompleteScreen.XpSlider.minValue = m_TotalXp;
            m_LevelCompleteScreen.XpSlider.maxValue = k_MilestoneFactor * (m_TotalXp + m_TempXp);
            m_LevelCompleteScreen.XpValue = m_TotalXp + m_TempXp;

            m_LevelCompleteScreen.CoinCount = m_TempCoins;

            m_TotalXp += m_TempXp;
            m_TempXp = 0f;
            SaveManager.Instance.XP = m_TotalXp;
        }

        void OnLose()
        {
            m_TempFood = 0;
            m_TotalXp += m_TempXp;
            m_TempXp = 0f;
            SaveManager.Instance.XP = m_TotalXp;
        }

        void Update()
        {
            if (m_Hud.gameObject.activeSelf)
            {
                m_TempXp += PlayerController.Instance.Speed * Time.deltaTime;
                m_Hud.XpValue = m_TempXp;

                if (SequenceManager.Instance.m_CurrentLevel is LoadLevelFromDef loadLevelFromDef)
                {
                    m_Hud.XpSlider.minValue = 0;
                    m_Hud.XpSlider.maxValue = loadLevelFromDef.m_LevelDefinition.LevelLength;
                }
            }
        }
    }
}
