using System;
using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace Investment.Property
{
    [MoonSharpUserData]
    public class ApartmentDictionary
    {
        public static event EventHandler<Apartment> OnApartmentAdded;
        public static event EventHandler<List<Apartment>> OnApartmentsAdded;

        public static Dictionary<int, Apartment> Apartments { get; private set; }

        public static void LoadApartments(List<Apartment> apartments)
        {
            foreach (var apartment in apartments)
                AddApartment(apartment);
            OnApartmentsAdded?.Invoke(null, apartments);
        }

        public static void AddApartment(Apartment apartment)
        {
            if (Apartments == null)
                Apartments = new Dictionary<int, Apartment>();

            if (Apartments.TryGetValue(apartment.id, out var _apartment))
                _apartment = apartment;
            else
                Apartments.Add(apartment.id, apartment);
            OnApartmentAdded?.Invoke(null, apartment);
        }
    }
}
