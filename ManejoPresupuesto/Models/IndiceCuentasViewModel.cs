namespace ManejoPresupuesto.Models
{
    public class IndiceCuentasViewModel
    {
        public string TipoCuenta { get; set; }

        public IEnumerable<Cuenta> Cuentas { get; set; }


        //Este balance sera igual a la suma de balances que esten en el IEnumerable de Cuenta
        public decimal Balance => Cuentas.Sum(x => x.Balance);

    }
}
