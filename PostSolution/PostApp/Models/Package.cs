﻿namespace PostApp.Models
{
    public class Package
    {
        public double WeightInGrams { get; set; }
        public int PaidPostFee { get; set; }
        public Receiver Receiver { get; set; }
        public AddressInformation ReceiverAddressInformation { get; set; }
        public Receiver ReturnReceiver { get; set; }
        public AddressInformation ReturnAddressInformation
        {
            get; set;
        }
    }
}