using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBaB.Services.Business
{
    public interface IWeaponBusiness<W>
    {
        /**
         * <summary>Gets a list of the inventory</summary>
         * <returns type="List<W>"></returns>
         */
        List<W> GetAllWeapons();

        /**
         * <summary>Add an item to the inventory</summary>
         * <param name="model" type="W"></param>
         */
        void AddWeaponInventory(W model);

        /**
         * <summary>Delete item from inventory</summary>
         * <param name="model" type="W"></param>
         */
        void DeleteWeaponFromInventory(W model);

        /**
         * <summary>Update information on an item in the inventory</summary>
         * <param name="model" type="W"></param>
         */
        void EditWeapon(W model);
    }
}
