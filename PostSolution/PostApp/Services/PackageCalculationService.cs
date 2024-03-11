using PostApp.Common;


namespace PostApp.Services
{
    public class PackageCalculationService
    {
        public double CalculateVolume(double lengthInCm, double heightInCm, double widthInCm)
        {
            return lengthInCm * heightInCm * widthInCm;
        }

        public double GetAddedMeasurements(double lengthInCm, double heightInCm, double widthInCm)
        {
            return lengthInCm + heightInCm + widthInCm;
        }

        public bool IsAbroad(string country)
        {
            return country != "SWEDEN";
        }

        public int CalculatePostageRate(double weightInGrams, double lengthInCm, double heightInCm, double widthInCm, bool abroad)
        {
            ValidateInput(weightInGrams, lengthInCm, heightInCm, widthInCm);

            var total = 0;
            total += PricingConstants.StartingFee;

            var weightPricing = GetWeightPrice(weightInGrams);
            total += weightPricing;

            var measurementPrice = GetMeasurementPrice(lengthInCm, widthInCm, heightInCm);
            total += measurementPrice;

            var reductionRatio = GetTotalPriceReductionRatio(lengthInCm, widthInCm, heightInCm, weightInGrams);
            total = (int)Math.Round(total * reductionRatio);
            return total;
        }

        private void ValidateInput(double weightInGrams, double lengthInCm, double heightInCm, double widthInCm)
        {
            if (weightInGrams <= 0 || weightInGrams > 25000)
            {
                throw new ArgumentException("Weight in grams must be greater than 0 and less than or equal to 25000.");
            }

            if (lengthInCm <= 0 || lengthInCm > 175)
            {
                throw new ArgumentException("Length must be over 0 and less than or equal to 175.");
            }

            if (heightInCm <= 0 || heightInCm > 175)
            {
                throw new ArgumentException("Height must be over 0 and less than or equal to 175.");
            }

            if (widthInCm <= 0 || widthInCm > 175)
            {
                throw new ArgumentException("Height must be over 0 and less than or equal to 175.");
            }
        }

        private int GetWeightPrice(double weightInGrams)
        {

            int cost = 0;

            if (weightInGrams > 100 && weightInGrams <= 500)
            {
                cost = 33;
            }

            if (weightInGrams > 500 && weightInGrams <= 1000)
            {
                cost = 52;
            }

            if (weightInGrams > 1000 && weightInGrams <= 2000)
            {
                cost = 78;
            }

            if (weightInGrams > 2000 && weightInGrams < 5000)
            {
                cost = 155;
            }

            if (weightInGrams > 5000 && weightInGrams <= 12500)
            {
                cost = 251;
            }

            if (weightInGrams >= 12500 && weightInGrams <= 25000)
            {
                cost = 373;
            }

            return cost;
        }

        private int GetMeasurementPrice(double lengthInCm, double widthInCm, double heightInCm)
        {
            if(lengthInCm >= 125 &&  lengthInCm <= 175)
            {
                return 111;
            }
            return 0;
        }

        private double GetTotalPriceReductionRatio(double lengthInCm, double widthInCm, double heightInCm, double weightInGrams)
        {
            if(weightInGrams > 5000 && lengthInCm == 80 && widthInCm == 60 && heightInCm == 20)
            {
                return 0.85;
            }

            return 1;
        }
    }
}
