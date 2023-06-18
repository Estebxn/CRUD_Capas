using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoDatos
{
    public class ClsDataBase
    {
        #region Variables Privadas

        private SqlConnection _objSqlConnection;
        private SqlDataAdapter _objSqlDataAdapter;
        private SqlCommand _objSqlCommand;
        private DataSet _dsResultados;
        private DataTable _dtParametros;
        private string _nombreTabla, _nombreSP, _mensajeErrorDB, _valorScalar, _nombreDB;
        private bool _scalar;

        #endregion

        #region Variables Públicas

        public SqlConnection ObjSqlConnection { get => _objSqlConnection; set => _objSqlConnection = value; }
        public SqlDataAdapter ObjSqlDataAdapter { get => _objSqlDataAdapter; set => _objSqlDataAdapter = value; }
        public SqlCommand ObjSqlCommand { get => _objSqlCommand; set => _objSqlCommand = value; }
        public DataSet DsResultados { get => _dsResultados; set => _dsResultados = value; }
        public DataTable DtParametros { get => _dtParametros; set => _dtParametros = value; }
        public string NombreTabla { get => _nombreTabla; set => _nombreTabla = value; }
        public string NombreSP { get => _nombreSP; set => _nombreSP = value; }
        public string MensajeErrorDB { get => _mensajeErrorDB; set => _mensajeErrorDB = value; }
        public string ValorScalar { get => _valorScalar; set => _valorScalar = value; }
        public string NombreDB { get => _nombreDB; set => _nombreDB = value; }
        public bool Scalar { get => _scalar; set => _scalar = value; }

        #endregion

        #region Constructores

        public ClsDataBase()
        {
            DtParametros = new DataTable("SpParametros");
            DtParametros.Columns.Add("Nombre");
            DtParametros.Columns.Add("TipoDato");
            DtParametros.Columns.Add("Valor");

            NombreDB = "DB_BasePruebas";


        }

        #endregion

        #region Métodos Privados

        private void CrearConexionBaseDatos(ref ClsDataBase objDataBase)
        {
            switch (objDataBase.NombreDB)
            {

                case "DB_BasePruebas":
                    objDataBase.ObjSqlConnection = new SqlConnection(Properties.Settings.Default.cadenaConeccion_DB_BasePruebas);
                    break;
                default:
                    break;
            }
        }

        private void ValidarConexionBaseDatos(ref ClsDataBase objDataBase)
        {
            if (objDataBase.ObjSqlConnection.State == ConnectionState.Closed)
            {
                objDataBase.ObjSqlConnection.Open();
            }
            else
            {
                objDataBase.ObjSqlConnection.Close();
                objDataBase.ObjSqlConnection.Dispose();
            }
        }

        private void AgregarParametros(ref ClsDataBase objDataBase)
        {
            if (objDataBase.DtParametros != null)
            {
                SqlDbType TipoDatoSQL = new SqlDbType();

                foreach (DataRow item in objDataBase.DtParametros.Rows)
                {
                    switch (item[1])
                    {
                        case "1":
                            TipoDatoSQL = SqlDbType.Bit;
                            break;

                        case "2":
                            TipoDatoSQL = SqlDbType.TinyInt;
                            break;

                        case "3":
                            TipoDatoSQL = SqlDbType.SmallInt;
                            break;

                        case "4":
                            TipoDatoSQL = SqlDbType.Int;
                            break;

                        case "5":
                            TipoDatoSQL = SqlDbType.BigInt;
                            break;

                        case "6":
                            TipoDatoSQL = SqlDbType.Decimal;
                            break;

                        case "7":
                            TipoDatoSQL = SqlDbType.SmallMoney;
                            break;

                        case "8":
                            TipoDatoSQL = SqlDbType.Money;
                            break;

                        case "9":
                            TipoDatoSQL = SqlDbType.Float;
                            break;

                        case "10":
                            TipoDatoSQL = SqlDbType.Real;
                            break;

                        case "11":
                            TipoDatoSQL = SqlDbType.Date;
                            break;

                        case "12":
                            TipoDatoSQL = SqlDbType.Time;
                            break;

                        case "13":
                            TipoDatoSQL = SqlDbType.SmallDateTime;
                            break;

                        case "14":
                            TipoDatoSQL = SqlDbType.DateTime;
                            break;

                        case "15":
                            TipoDatoSQL = SqlDbType.Char;
                            break;

                        case "16":
                            TipoDatoSQL = SqlDbType.NChar;
                            break;

                        case "17":
                            TipoDatoSQL = SqlDbType.VarChar;
                            break;

                        case "18":
                            TipoDatoSQL = SqlDbType.NVarChar;
                            break;

                        default:
                            break;
                    }

                    if (objDataBase.Scalar)
                    {
                        if (item[2].ToString().Equals(string.Empty))
                        {
                            objDataBase.ObjSqlCommand.Parameters.Add(item[0].ToString(), TipoDatoSQL).Value = DBNull.Value;
                        }
                        else
                        {
                            objDataBase.ObjSqlCommand.Parameters.Add(item[0].ToString(), TipoDatoSQL).Value = item[2].ToString();
                        }
                    }
                    else
                    {
                        if (item[2].ToString().Equals(string.Empty))
                        {
                            objDataBase.ObjSqlDataAdapter.SelectCommand.Parameters.Add(item[0].ToString(), TipoDatoSQL).Value = DBNull.Value;
                        }
                        else
                        {
                            objDataBase.ObjSqlDataAdapter.SelectCommand.Parameters.Add(item[0].ToString(), TipoDatoSQL).Value = item[2].ToString();
                        }
                    }
                }

            }
        }

        private void PrepararConexionBaseDatos(ref ClsDataBase objDataBase)
        {
            CrearConexionBaseDatos(ref objDataBase);
            ValidarConexionBaseDatos(ref objDataBase);
        }

        private void EjecutarDataAdapter(ref ClsDataBase objDataBase)
        {
            try
            {
                PrepararConexionBaseDatos(ref objDataBase);

                objDataBase.ObjSqlDataAdapter = new SqlDataAdapter(objDataBase.NombreSP, objDataBase.ObjSqlConnection);
                objDataBase.ObjSqlDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                AgregarParametros(ref objDataBase);
                objDataBase.DsResultados = new DataSet();
                objDataBase.ObjSqlDataAdapter.Fill(objDataBase.DsResultados, objDataBase.NombreTabla);
            }
            catch (Exception ex)
            {

                objDataBase.MensajeErrorDB = ex.Message.ToString();

            }
            finally
            {
                if (objDataBase.ObjSqlConnection.State == ConnectionState.Open)
                {
                    ValidarConexionBaseDatos(ref objDataBase);
                }
            }
        }

        private void EjecutarCommand(ref ClsDataBase objDataBase)
        {
            try
            {
                PrepararConexionBaseDatos(ref objDataBase);
                objDataBase.ObjSqlCommand = new SqlCommand(objDataBase.NombreSP, objDataBase.ObjSqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                AgregarParametros(ref objDataBase);

                if (objDataBase.Scalar)
                {
                    objDataBase.ValorScalar = objDataBase.ObjSqlCommand.ExecuteScalar().ToString().Trim();
                }
                else
                {
                    objDataBase.ObjSqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {

                objDataBase.MensajeErrorDB = ex.Message.ToString();

            }
            finally
            {
                if (objDataBase.ObjSqlConnection.State == ConnectionState.Open)
                {
                    ValidarConexionBaseDatos(ref objDataBase);
                }
            }
        }

        #endregion

        #region Métodos Públicos

        public void CRUD(ref ClsDataBase objDataBase)
        {
            if (objDataBase.Scalar)
            {
                EjecutarCommand(ref objDataBase);
            }
            else
            {
                EjecutarDataAdapter(ref objDataBase);
            }
        }

        #endregion

    }
}
