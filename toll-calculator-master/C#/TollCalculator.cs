using System;
using System.Globalization;
using TollFeeCalculator;

namespace TollFeeCalculator //Lade till namespace
{
public class TollCalculator
{

    /**
     * Calculate the total toll fee for one day
     *
     * @param vehicle - the vehicle
     * @param dates   - date and time of all passes on one day
     * @return - the total toll fee for that day
     * 
     * The maximum fee for one day is 60 SEK
     * A vehicle should only be charged once an hour
     * In the case of multiple fees in the same hour period, the highest one applies.
     */
    // Lagt till enkel felhentering på alla metoder.
    public int GetTollFee(Vehicle vehicle, DateTime[] dates)
    {
        try
        {
            DateTime intervalStart = dates[0];
            TimeSpan starttime = TimeSpan.FromHours(1);
            for (int i = 0; i < 5; i++) // Populera listan med datum.
            {
                intervalStart.Add(starttime);
            }
            
            int totalFee = 0;
            foreach (DateTime date in dates)
            {
                int nextFee = GetTollFee(date, vehicle);
                int tempFee = GetTollFee(intervalStart, vehicle);

                long diffInMillies = date.Millisecond - intervalStart.Millisecond;
                long minutes = (diffInMillies/1000)/60; //Lade till parentes

                if (minutes <= 60)
                {
                    if (totalFee > 0) totalFee -= tempFee;
                    if (nextFee >= tempFee) tempFee = nextFee;
                    totalFee += tempFee;
                }
                else
                {
                    totalFee += nextFee;
                }
            }
            if (totalFee > 60) totalFee = 60;
            return totalFee;
        }
        catch (Exception e)
        {

            Console.WriteLine("Något gick fel.", e);
        }
        return 0;
    }

    private bool IsTollFreeVehicle(Vehicle vehicle) // Tar emot ett värde för att kolla om fordonet ska beskattas eller ej.
    {       
        try
        {
            if (vehicle == null) return false;
            string vehicleType = vehicle.GetVehicleType(); //Ändrade String till string
            return vehicleType.Equals(TollFreeVehicles.Motorbike.ToString()) ||
                    vehicleType.Equals(TollFreeVehicles.Tractor.ToString()) ||
                    vehicleType.Equals(TollFreeVehicles.Emergency.ToString()) ||
                    vehicleType.Equals(TollFreeVehicles.Diplomat.ToString()) ||
                    vehicleType.Equals(TollFreeVehicles.Foreign.ToString()) ||
                    vehicleType.Equals(TollFreeVehicles.Military.ToString());
        }
        catch (Exception e)
        {
            Console.WriteLine("Något gick fel.", e);
        }
        return false;
    }

    public int GetTollFee(DateTime date, Vehicle vehicle)
    {
        try
        {
            if (IsTollFreeDate(date) || IsTollFreeVehicle(vehicle)) return 0;

            int hour = date.Hour;
            int minute = date.Minute;
            // Kortade ned antalet urval i if-satsen
            if ((hour == 6 && minute >= 0 && minute <= 29) || (hour >= 8 && hour <= 14 && minute >= 30 && minute <= 59) || (hour == 18 && minute >= 0 && minute <= 29)) return 8;
            else if ((hour == 6 && minute >= 30 && minute <= 59) || (hour == 8 && minute >= 0 && minute <= 29) || (hour == 15 && minute >= 0 && minute <= 29) || (hour == 17 && minute >= 0 && minute <= 59)) return 13;
            else if ((hour == 7 && minute >= 0 && minute <= 59) || (hour == 15 && minute >= 0 || hour == 16 && minute <= 59)) return 18;
            else return 0;
        }
        catch (Exception e)
        {

            Console.WriteLine("Något gick fel.", e);
        }
        return 0;
    }

    private Boolean IsTollFreeDate(DateTime date) // Kollar om det är ett datum där det inte ska lägga på en fee.
    {
        try
        {
            int year = date.Year;
            int month = date.Month;
            int day = date.Day;

            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) return true; //Kollar ifall det är helgdag. 

            if (year == 2013)
            { // Datumen verkar stämma. Antingen röda dagar eller helgdagar. Ska returnera true.
                if (month == 1 && day == 1 ||
                    month == 3 && (day == 28 || day == 29) ||
                    month == 4 && (day == 1 || day == 30) ||
                    month == 5 && (day == 1 || day == 8 || day == 9) ||
                    month == 6 && (day == 5 || day == 6 || day == 21) ||
                    month == 7 ||
                    month == 11 && day == 1 ||
                    month == 12 && (day == 24 || day == 25 || day == 26 || day == 31))
                {
                    return true;
                }
            }
            return false;
        }
        catch (Exception e)
        {

            Console.WriteLine("Något gick fel.", e);
        }
        return false;
    }

    private enum TollFreeVehicles // Alla typer av fordon som inte ska beskattas.
    {
        Motorbike = 0,
        Tractor = 1,
        Emergency = 2,
        Diplomat = 3,
        Foreign = 4,
        Military = 5
    }
}
}