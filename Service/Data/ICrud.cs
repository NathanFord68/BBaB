using BBaB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBaB.Service.Data
{
    /**
     * <typeparam name="T"></typeparam>
     */
    public interface ICrud<T>
    {
        /**
         * <summary>Pass in a user/principal model to register the user to the database. 
         * Ensure any passwords are encrypted with a 64 character encryption</summary>
         * <param name="model" type="T">Model to be created in db</param>
         * <exception cref="RecordNotCreatedException"></exception>
         */
        void CreateT(T model);

        /**
         * <summary>Pass in the T model, will find said model by the id property</summary>
         * <param name="id" type="int"></param>
         * <returns type="T"></returns>
         * <exception cref="RecordNotFoundException"></exception>
         */
        T ReadTById(int id);


        /**
         * <summary>Pass in a low and a high value to read a certain range of data</summary>
         * <param name="low">Designates the lowest number row to read</param>
         * <param name="high">Designates the highest number row to read</param>
         * <returns type="List<T>">List of objects</returns>
         * <exception cref="RecordNotFoundException"></exception>
         */
        List<T> ReadBetweenT(int low, int high);

        /**
         * <summary>Find the models info based on a designated field</summary>
         * <param name="model" type="T"></param>
         * <returns type="T"></returns>
         * <exception cref="RecordNotFoundException"></exception>
         */
        T ReadTByField(T model);

        /**
         * <summary>Reads a list of all from the database</summary>
         * <returns type="List<T>">List of Objects</returns>
         * <exception cref="RecordNotFoundException"></exception>
         */
        List<T> ReadAllT();

        /**
         * <summary>Pass in a model to update it in the database, id field must have value</summary>
         * <param name="model" type="T">Model to update</param>
         * <exception cref="RecordNotUpdatedException"></exception>
         */
        void UpdateT(T model);

        /**
         * <summary>Pass model with a unique field to be deleted.</summary>
         * <param name="model" type="T">Model to be deleted</param>
         * <exception cref="RecordNotDeletedException"></exception>
         */
        void DeleteT(T model);
    }
}
