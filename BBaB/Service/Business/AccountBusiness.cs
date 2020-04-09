﻿using BBaB.Models;
using BBaB.Service.Data;
using BBaB.Service.Exceptions;
using BBaB.Services.Data;
using BBaB.Utility;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using BBaB.Utility.Interfaces;

namespace BBaB.Service.Business
{
    /**
     * TODO DI the data layer classes
     */
    public class AccountBusiness : IAccountBusiness<PrincipalModel, AddressModel>
    {
        private IDBConnect<SqlConnection> _connect;
        private IBBaBLogger logger;

        public AccountBusiness(IDBConnect<SqlConnection> connect, IBBaBLogger logger)
        {
            this._connect = connect;
            this.logger = logger;
        }


        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public void AddToCart(CartModel cartItem)
        {
            this.logger.Info("Entering AccountBusiness.AddToCart to add an item for");

            this.logger.Info("Getting a connection.");
            using (SqlConnection connection = _connect.GetConnection())
            {
                this.logger.Info("Opening a connection to the database");
                connection.Open();

                this.logger.Info("Getting data layer CartData");
                ICrud<CartModel> cartData = new CartData(connection, this.logger);

                this.logger.Info("Passing cart item to be added");
                cartData.CreateT(cartItem);

                this.logger.Info("Closing the database connection");
                connection.Close();
            }

            this.logger.Info("Leaving AccountBusiness@AddToCart");
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public PrincipalModel AuthenticateAccount(PrincipalModel model)
        {
            this.logger.Info("Entering AccountBusiness@AuthenticateAccount");
            PrincipalModel account;
            //Get the connection
            this.logger.Info("Getting a connection to the database");
            using (SqlConnection connection = _connect.GetConnection())
            {
                this.logger.Info("Opening the connection to the database");
                connection.Open();

                //Instantiate the data layer
                this.logger.Info("Getting data layer AccountData");
                ICrud<PrincipalModel> accountData = new AccountData(connection, this.logger);

                //Get the database account for this user.
                this.logger.Info("Getting the recorded model for this user");
                account = accountData.ReadTByField(model);

                //Salt the logging in users password
                this.logger.Info("Salting the inputed password for comparison");
                model._credentials._password = new StringManipulator(this.logger).HashPassword(account._salt + model._credentials._password);

                //Compare passwords
                this.logger.Info("Comparing the passwords to authenticate account");
                if (model._credentials._password.Equals(account._credentials._password))
                {
                    this.logger.Info("Nullifying salt to prevent it from reaching the presentation layer");
                    account._salt = null;
                }
                else
                {
                    this.logger.Info("Closing the connection before throwing a failed login exception");
                    connection.Close();

                    this.logger.Info("Throwing a login unseccessful exception to signal an unauthenticated user");
                    throw new AuthenticationFailedException("Login was unsuccessful");
                }

                this.logger.Info("Closing the connection to the database");
                connection.Close();

            }

            //Return the model
            this.logger.Info("Returning account model from AccountBusiness@AuthenticateUser with successful authentication");
            return account;
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public void DeleteAccount(PrincipalModel model)
        {
            this.logger.Info("Entering AccountBusiness@DeleteAccount");
            this.logger.Info("Getting a connection to the database");
            using (SqlConnection connection = _connect.GetConnection())
            {
                this.logger.Info("Opening a connection to the database");
                connection.Open();

                this.logger.Info("Getting data layer AccountData");
                ICrud<PrincipalModel> crud = new AccountData(connection, this.logger);

                this.logger.Info("Passing model to be deleted in the data layer");
                crud.DeleteT(model);

                this.logger.Info("Closing connection to the database");
                connection.Close();
            }

            this.logger.Info("Returning from AccountBusiness@DeleteAccount with successful deletion of account");
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public AddressModel GetAddress(PrincipalModel model)
        {
            this.logger.Info("Entering AccountBusiness@GetAddress");

            //Generate model that needs populating
            AddressModel address = new AddressModel();
            //Store user in model for query
            address._resident = model;

            this.logger.Info("Getting a connection to the database");
            using (SqlConnection connection = _connect.GetConnection())
            {
                this.logger.Info("Opening connection to the database");
                connection.Open();
                //Get the data layer

                this.logger.Info("Getting data layer AddressData");
                ICrud<AddressModel> addressData = new AddressData(connection, this.logger);

                //Pass the model to get the data
                this.logger.Info("Passing model to get the address of this user");
                address = addressData.ReadTByField(address);

                //close the connection
                this.logger.Info("Closing the connection to the database");
                connection.Close();
            }

            this.logger.Info("Returning the address model of the user");
            return address;
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public CartModel GetCart(PrincipalModel model)
        {
            this.logger.Info("Entering AccountBusiness@GetCart");
            //Declare the model
            CartModel cart;

            //Get the connection
            this.logger.Info("Getting a connection to the database");
            using (SqlConnection connection = _connect.GetConnection())
            {
                this.logger.Info("Opening a connection to the database");
                connection.Open();

                //Instantiate the data layer
                this.logger.Info("Getting data layer CartData");
                ICrud<CartModel> cartData = new CartData(connection, this.logger);

                //Make the cart
                this.logger.Info("Passing model to retrieve the cart data from the data layer");
                cart = cartData.ReadTById(model._id);

                //Close the connection
                this.logger.Info("Closing connection to the database");
                connection.Close();
            }

            //Return the cart
            this.logger.Info("Returning the model from AccountBusiness@GetCart");
            return cart;
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public InvoiceModel ProcessPurchase(CartModel cart)
        {
           this.logger.Info("Entering AccountBusiness@ProcessPurchase");

            //Create the invoice model
            InvoiceModel invoice = new InvoiceModel();

            //Get connection
           this.logger.Info("Getting a connection to the database");
            using (SqlConnection connection = _connect.GetConnection())
            {
               this.logger.Info("Opening connection to the database");
                connection.Open();

                //Create the transaction
               this.logger.Info("Declaring a transaction for the database");
                SqlTransaction transaction;

                //Start the transaction
               this.logger.Info("Initializing/Beginning transaction");
                transaction = connection.BeginTransaction();

                //Create the data layer
               this.logger.Info("Getting data layer InvoiceData, CartData, AddressData, WeaponData");
                ICrud<InvoiceModel> invoiceData = new InvoiceData(connection, this.logger);
                ICrud<CartModel> cartData = new CartData(connection, this.logger);
                ICrud<AddressModel> addressData = new AddressData(connection, this.logger);
                ICrud<WeaponModel> weaponData = new WeaponData(connection, this.logger);

                try
                {
                    //Populate the address in the invoice
                   this.logger.Info("Populating address information of the invoice");
                    AddressModel address = new AddressModel();
                    address._resident = cart._customer;

                    invoice._address = addressData.ReadTByField(address);

                    //Populate the customer in the invoice
                   this.logger.Info("Populating customer information of the invoice");
                    invoice._customer = cart._customer;

                    //Populate the weapons in the invoice
                   this.logger.Info("Populating the weapon information of the invoice");
                    invoice._weapons = cart._weapons;


                    //Process transaction
                   this.logger.Info("Passing invoice record to the data layer");
                    invoiceData.CreateT(invoice);

                   this.logger.Info("Looping through Weaopon list to Move item Delete from inventory");
                    foreach (WeaponModel w in cart._weapons)
                    {
                        //Assign each weapon to be updated.
                        cart._weaponToUpdate = w;

                        //Update cart and inventory
                        weaponData.DeleteT(cart._weaponToUpdate);
                    }

                   this.logger.Info("Deleting cart object");
                    cartData.DeleteT(cart);

                   this.logger.Info("Commiting the transaction");
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    this.logger.Error(cart._customer._credentials._email, "Catching Exception ", e);

                    this.logger.Warning(cart._customer._credentials._email, "Rolling back transaction");
                    transaction.Rollback();

                   this.logger.Info("Closing connection to database after failed transaction");
                    connection.Close();

                   this.logger.Info("Throwing exception back to the presentation layer");
                    throw new TransactionFailedException("The transaction failed.", e.InnerException);
                }
               this.logger.Info("Closing connection to the database");
                connection.Close();
            }

            //Return the invoice
           this.logger.Info("Returning invoice model after a successful transaction in AccountBusiness@ProcessPurchase");
            return invoice;
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public void UpdateAccount(PrincipalModel model)
        {
            this.logger.Info("Entering AccountBusiness@UpdateAccount");

            //Get a connection
            this.logger.Info("Getting a connection to the database");
            using (SqlConnection connection = _connect.GetConnection())
            {
                this.logger.Info("Opening connection to the database");
                connection.Open();

                //Get data layer
                this.logger.Info("Getting data lyaer AccountData");
                ICrud<PrincipalModel> accountData = new AccountData(connection, this.logger);

                //Update account
                this.logger.Info("Passing model to be updated in data layer");
                accountData.UpdateT(model);

                //Close connection
                this.logger.Info("Closing connection to the database");
                connection.Close();
            }
            this.logger.Info("Exiting AccountBusiness@UpdateAccount");
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public void RegisterAccount(PrincipalModel model)
        {
            this.logger.Info("Entering AccountBusiness@RegisterAccount");

            //Get a String Manipulator class
            StringManipulator strManip = new StringManipulator(this.logger);

            //Get a salt value
            this.logger.Info("Creating a new salt value for account");
            model._salt = strManip.GenerateString(64);

            //Hash password with salt
            this.logger.Info("Hashing password with salt");
            model._credentials._password = strManip.HashPassword(model._salt + model._credentials._password);

            //Get connection to database
            this.logger.Info("Getting a connection to the database");
            using (SqlConnection connection = _connect.GetConnection())
            {
                this.logger.Info("Opening connection to database");
                connection.Open();

                //Get data layer
                this.logger.Info("Getting data layer AccountData");
                ICrud<PrincipalModel> accountData = new AccountData(connection, this.logger);

                //Send data to be processed
                this.logger.Info("Passing model to be registered in data layer");
                accountData.CreateT(model);

                //Close the connection
                this.logger.Info("Closing connection to the database");
                connection.Close();
            }

            this.logger.Info("Exiting AccountBusiness@RegisterAccount");
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public void RegisterAddress(AddressModel model)
        {
            this.logger.Info("Entering AccountBusiness@RegisterAddress");

            //Get a connection
            this.logger.Info("Getting a connection to the database");
            using (SqlConnection connection = _connect.GetConnection())
            {
                this.logger.Info("Opening a connection to the database");
                connection.Open();

                //Get the data layer
                this.logger.Info("Getting data layer AddressData");
                ICrud<AddressModel> addressData = new AddressData(connection, this.logger);

                //Pass model to be stored
                this.logger.Info("Passing model to the data layer");
                addressData.CreateT(model);

                //Close the connection
                this.logger.Info("Closing connection to the database");
                connection.Close();
            }
            this.logger.Info("Exiting AccountBusiness@RegisterAddress");
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        //TODO fix this method
        public void RemoveFromCart(WeaponModel weapon)
        {
            /*
             * Reha if your looking at this. I'm ommiting logging statements cause I 
             * realized this method wouldn't work even if we used it. I'm adding it 
             * to list of things to do for the clean up milestone
             */
            //Create the cart with the weapon to be updated
            CartModel cart = new CartModel();
            cart._weaponToUpdate = weapon;

            //Get a connection
            using (SqlConnection connection = _connect.GetConnection())
            {
                connection.Open();
                //Get the data layer
                ICrud<CartModel> cartData = new CartData(connection, this.logger);

                //Delete the item from the cart
                cartData.DeleteT(cart);

                //Close the connection
                connection.Close();
            }
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public void UpdateAddress(AddressModel model)
        {
            this.logger.Info("Entering AccountBusiness@UpdateAddress");

            //Get a connection
            this.logger.Info("Getting a connection to the database");
            using (SqlConnection connection = _connect.GetConnection())
            {
                this.logger.Info("Opening connection to the database");
                connection.Open();

                //Get the data layer
                this.logger.Info("Getting data layer AddressData");
                ICrud<AddressModel> addressData = new AddressData(connection, this.logger);

                //Update the model
                this.logger.Info("Passing model to be updated");
                addressData.UpdateT(model);

                //Close the connection
                this.logger.Info("Closing connection to the database");
                connection.Close();
            }
            this.logger.Info("Exiting AccountBusiness@UpdateAddress");
        }

        /**
         * <see cref="IAccountBusiness{P, A}"/>
         */
        public InvoiceModel ViewInvoice(PrincipalModel customer, DateTime date)
        {
            this.logger.Info("Entering AccountBusiness@ViewInvoice");

            //Declare the invoice
            InvoiceModel invoice = new InvoiceModel();
            invoice._customer = customer;
            invoice._dateTime = date;

            //Get a connection
            this.logger.Info("Getting connection to the database");
            using (SqlConnection connection = _connect.GetConnection())
            {
                this.logger.Info("Opening connection to the database");
                connection.Open();

                //Get the data layer
                this.logger.Info("Getting data layer InvoiceData");
                ICrud<InvoiceModel> invoiceData = new InvoiceData(connection, this.logger);

                //Get the invoice for this user
                this.logger.Info("Retriving the invoice from the data layer");
                invoice = invoiceData.ReadTByField(invoice);

                //Close the connection
                this.logger.Info("Closing connection to the database");
                connection.Close();
            }

            //Return the invoice
            this.logger.Info("Returning invoice model from AccountBusiness@ViewInvoice");
            return invoice;
        }

        /**
         * <see cref="IAccountBusiness"/>
         */
        public PrincipalModel UpdatePassword(PrincipalModel model, string newp)
        {
            this.logger.Info("Entering AccountBusiness@UpdatePassword");

            //Get a connection
            this.logger.Info("Getting connection to the database");
            using (SqlConnection connection = _connect.GetConnection())
            {
                //Open the connection
                this.logger.Info("Opening connection to the database");
                connection.Open();

                //Get the data layer
                this.logger.Info("Getting data layer AccountData");
                ICrud<PrincipalModel> accountData = new AccountData(connection, this.logger);

                ///Authenticate the old password
                //
                //Get the data profile
                this.logger.Info("Getting users old information from the data layer");
                PrincipalModel account = accountData.ReadTByField(model);

                //Create the string manipulator
                StringManipulator sm = new StringManipulator(this.logger);
                //Hash the old password
                this.logger.Info("Hashing the user's old password");
                model._credentials._password = sm.HashPassword(account._salt + model._credentials._password);

                //Check passwords match
                this.logger.Info("Confirmin password match");
                if (account._credentials._password.Equals(model._credentials._password))
                {
                    //if they do, update the password
                    this.logger.Info("Update user's account with new password");
                    model._credentials._password = sm.HashPassword(account._salt + newp);

                    this.logger.Info("Passing model to be updated in the database");
                    accountData.UpdateT(model);

                    this.logger.Info("Set salt value to empty to keep it from presentation layer");
                    model._salt = "";
                }
                else
                {
                    this.logger.Info("Closing connection to the database after failed authentication");
                    connection.Close();

                    this.logger.Info("Throwing exception to presentation layer");
                    throw new AuthenticationFailedException("Incorrect old password");
                }

                //close the connection
                this.logger.Info("Closing connection to the database");
                connection.Close();
            }

            this.logger.Info("Returning new model from AccountBusiness@UpdatePassword");
            return model;
        }
    }
}