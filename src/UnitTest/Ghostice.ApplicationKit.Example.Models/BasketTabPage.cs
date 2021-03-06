﻿using Ghostice.Core;
using Ghostice.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ghostice.ApplicationKit.Example.Models
{
    public class BasketTabPage : WinFormControlBase
    {

        [ControlDescriptor("SelectPurchases", "Name=cmbPetBasket")]
        private PlaceHolder<WinFormComboBox> selectPurchases;

        [ControlDescriptor("BasketPurchases", "Name=lstvewBasketPurchases")]
        private PlaceHolder<WinFormListView> basketPurchases;

        [ControlDescriptor("Cash", "Name=rdobutCash")]
        private PlaceHolder<WinFormRadioButton> radioCash;

        [ControlDescriptor("Cheque", "Name=rdobutCheque")]
        private PlaceHolder<WinFormRadioButton> radioCheque;

        [ControlDescriptor("Card", "Name=rdobutCard")]
        private PlaceHolder<WinFormRadioButton> radioCard;

        [ControlDescriptor("VATReceipt", "Name=chkVATReceipt")]
        private PlaceHolder<WinFormCheckBox> vatReceipt;

        [ControlDescriptor("TotalValue", "Name=lblTotalValue")]
        private PlaceHolder<WinFormLabel> totalValue;

        [ControlDescriptor("BasketReset", "Name=butBasketReset")]
        private PlaceHolder<WinFormButton> basketReset;

        [ControlDescriptor("BasketPurchase", "Name=butBasketPurchase")]
        private PlaceHolder<WinFormButton> basketPurchase;

        public BasketTabPage(InterfaceControl Parent)
            : base(Parent)
        {

        }

        public WinFormComboBox SelectPurchaes
        {
            get
            {
                return selectPurchases.GetControl();
            }
        }

        public WinFormListView BasketPurchases
        {
            get
            {
                return basketPurchases.GetControl();
            }
        }

        public WinFormRadioButton Cash
        {
            get
            {
                return radioCash.GetControl();
            }
        }

        public WinFormRadioButton Cheque
        {
            get
            {
                return radioCheque.GetControl();
            }
        }

        public WinFormRadioButton Card
        {
            get
            {
                return radioCard.GetControl();
            }
        }

        public WinFormCheckBox VATReceipt
        {
            get { return vatReceipt.GetControl(); }
        }

        public WinFormLabel Total
        {
            get { return totalValue.GetControl(); }
        }

        public WinFormButton Reset
        {
            get { return basketReset.GetControl(); }
        }

        public WinFormButton Purchase
        {
            get { return basketPurchase.GetControl(); }
        }

    }
}
