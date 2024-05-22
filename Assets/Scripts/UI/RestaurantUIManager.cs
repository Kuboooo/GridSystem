using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace UI {
    public class RestaurantUIManager : MonoBehaviour {

        public static event Action<int> OnToppingsSelected;


        [SerializeField] private Button savePizza;
        [SerializeField] private Button cancelPizza;

        [SerializeField] private Button topping1;
        [SerializeField] private Button topping2;
        [SerializeField] private Button topping3;
        [SerializeField] private Button topping4;
        [SerializeField] private Button topping5;
        [SerializeField] private Button topping6;
        [SerializeField] private Button topping7;
        [SerializeField] private Button topping8;
        [SerializeField] private Button topping9;

        private string selectedTopping1;
        private string selectedTopping2;


        private void Start() {
            savePizza.onClick.AddListener(SaveRecipe);
            cancelPizza.onClick.AddListener(CancelRecipe);

            topping1.onClick.AddListener(() => { SelectTopping(topping1, ToppingsOrganizer.Mushrooms); });
            topping2.onClick.AddListener(() => { SelectTopping(topping2, ToppingsOrganizer.Corn); });
            topping3.onClick.AddListener(() => { SelectTopping(topping3,ToppingsOrganizer.Olives); });
            topping4.onClick.AddListener(() => { SelectTopping(topping4,ToppingsOrganizer.Salami); });
            topping5.onClick.AddListener(() => { SelectTopping(topping5,ToppingsOrganizer.Prosciutto); });
            topping6.onClick.AddListener(() => { SelectTopping(topping6,ToppingsOrganizer.Artichokes); });
            topping7.onClick.AddListener(() => { SelectTopping(topping7,ToppingsOrganizer.Tuna); });
            topping8.onClick.AddListener(() => { SelectTopping(topping8,ToppingsOrganizer.Bacon); });
            topping9.onClick.AddListener(() => { SelectTopping(topping9,ToppingsOrganizer.Pineapple); });
        }

        private void SelectTopping(Button toppingButton, string topping) {
            if (selectedTopping1 == topping || selectedTopping2 == topping) {
                RemoveTopping(topping);
                SetButtonColor(toppingButton, new Color(1f, 1f, 1f));
            }
            else {
                AddTopping(topping, toppingButton);
            }
        }

        private static void SetButtonColor(Button toppingButton, Color color) {
            toppingButton.GetComponent<Image>().color = color;
        }

        private void AddTopping(string topping, Button toppingButton) {
            Debug.Log("Topping  " + topping + " selected");
            if (selectedTopping1 is null) {
                selectedTopping1 = topping;
                toppingButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                Debug.Log("Topping 1 " + topping + " selected");
            } else if (selectedTopping2 is null) {
                selectedTopping2 = topping;
                toppingButton.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                Debug.Log("Topping 2 " + topping + " selected");
            }
            else {
                Debug.Log("Cannot select " + topping + " both toppings already selected");
            }
        }
        private void RemoveTopping(string topping) {
            Debug.Log("Topping  " + topping + " selected");
            if (selectedTopping1 == topping) {
                selectedTopping1 = null;
                Debug.Log("Topping 1 " + topping + " removed");
            } else if (selectedTopping2 == topping) {
                selectedTopping2 = null;
                Debug.Log("Topping 2 " + topping + " removed");
            }
            Debug.Log("Cannot remove " + topping + " both toppings already removed");
        }

        private void SaveRecipe() {
            int combinationValue = ToppingsOrganizer.GetCombinationValue(selectedTopping1, selectedTopping2);
            Debug.Log("Saving toppings " + selectedTopping1 + " and " + selectedTopping2 + " with value " + combinationValue);
            OnToppingsSelected?.Invoke(combinationValue);
        }

        private void CancelRecipe() {
            Debug.Log("Cancelling toppings " + selectedTopping1 + " and " + selectedTopping2);
            selectedTopping1 = null;
            selectedTopping2 = null;
            Color color = new Color(1f, 1f, 1f);
            SetButtonColor(topping1, color);
            SetButtonColor(topping2, color);
            SetButtonColor(topping3, color);
            SetButtonColor(topping4, color);
            SetButtonColor(topping5, color);
            SetButtonColor(topping6, color);
            SetButtonColor(topping7, color);
            SetButtonColor(topping8, color);
            SetButtonColor(topping9, color);
        }
    }
}