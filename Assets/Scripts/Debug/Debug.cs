using TMPro;
using System;
using System.Linq;
using UnityEngine;
using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace REE.Debug
{
    [MoonSharpUserData]
    public class Debug : MonoBehaviour
    {
        public static Debug Instance { get; private set; }
        public static event EventHandler<State> OnToggled;

        public static State State { get; private set; }

        private TextMeshProUGUI headerText;
        private TextMeshProUGUI listText;
        private Transform listContent;
        private Transform listTransform;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        private void Start()
        {
            State = new State();

            listTransform = transform.Find("GameMenu/DebugMenu/List");
            listContent = listTransform.Find("Scroll View/Viewport/Content");
            listText = listContent.Find("StringPrefab").gameObject.GetComponent<TextMeshProUGUI>();
            headerText = listTransform.Find("Header").gameObject.GetComponent<TextMeshProUGUI>();
        }

        public void Set(bool active)
        {
            State.IsActive = active;
            OnToggled?.Invoke(this, State);
        }

        public void ListEmployees(string company, int page)
        {
            if (!State.IsActive) return;

            List<Employment.Employment> employments =
                Employment.EmploymentDictionary.Employments[company].Values
                .Skip(page * 100).Take(100).ToList();

            List<Person.Employee.Employee> employees =
                Person.Employee.EmployeeDictionary.SafeGetEmployees(employments.Select(x => x.employee_id).ToList());

            List(employees);
            headerText.text += $" - {company} - Page {page} of {Mathf.CeilToInt(Employment.EmploymentDictionary.Employments[company].Values.Count / 100)}";
        }

        public void ListApartments(int page)
        {
            if (!State.IsActive) return;

            List<Investment.Property.Apartment> apartments = 
                Investment.Property.ApartmentDictionary.Apartments.Values.
                Skip(page * 100).Take(100).ToList();

            List(apartments);
            headerText.text += $" - Page {page} of {Mathf.CeilToInt(Investment.Property.ApartmentDictionary.Apartments.Count / 100)}";
        }

        public void ListBuildings(int page)
        {
            if (!State.IsActive) return;

            List<Investment.Property.Building> buildings =
                Investment.Property.BuildingDictionary.Buildings.Values.SelectMany(x => x.Values).
                Skip(page * 100).Take(100).ToList();

            List(buildings);
            headerText.text += $" - Page {page}";
        }

        public void ListJobs(int page)
        {
            if (!State.IsActive) return;

            List<Job.Job> jobs =
                Job.JobDictionary.Jobs.Values.
                Skip(page * 100).Take(100).ToList();

            List(jobs);
            headerText.text += $" - Page {page} of {Mathf.CeilToInt(Job.JobDictionary.Jobs.Count / 100)}";
        }

        public void ListEmployments(int page)
        {
            if (!State.IsActive) return;

            List<Employment.Employment> employments =
                Employment.EmploymentDictionary.Employments.Values.SelectMany(x => x.Values).
                Skip(page * 100).Take(100).ToList();

            List(employments);
            headerText.text += $" - Page {page}";
        }

        public void ListNames(int page)
        {
            if (!State.IsActive) return;

            List<string> names =
                Person.Constants.Values.names.
                Skip(page * 100).Take(100).ToList();

            List(names);
            headerText.text += $" - Page {page} of {Mathf.CeilToInt(Person.Constants.Values.names.Count / 100)}";
        }

        public void ListSurnames(int page)
        {
            if (!State.IsActive) return;

            List<string> names =
                Person.Constants.Values.surnames.
                Skip(page * 100).Take(100).ToList();

            List(names);
            headerText.text += $" - Page {page} of {Mathf.CeilToInt(Person.Constants.Values.surnames.Count / 100)}";
        }

        public void List<T>(List<T> list) where T : class
        {
            if (!State.IsActive) return;

            headerText.text = $"{typeof(T)}";
            listText.text = "";

            int i;
            for (i = 0; i < list.Count; i++)
            {
                listText.text += $"\n{list[i]}";
            }

            listText.rectTransform.sizeDelta = 
                new Vector2(listText.rectTransform.sizeDelta.x, listText.preferredHeight);
            listText.ForceMeshUpdate();
            listTransform.gameObject.SetActive(true);
        }
    }
}
