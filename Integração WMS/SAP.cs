// Decompiled with JetBrains decompiler
// Type: Integração.WMS.SAP
// Assembly: Integração WMS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6E836F85-7F14-4ACE-B260-A3ABAFE0C808
// Assembly location: C:\Program Files (x86)\WMS\Integração\Integração WMS.exe

using log4net;
using SAPbobsCOM;
using System;
using System.Configuration;
using System.Runtime.InteropServices;


namespace Integração.WMS
{
  public class SAP
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof (SAP));

    public static Company oCompany { get; set; }

    public static void Connect()
    {
      SAP.oCompany = (Company) new CompanyClass();
      try
      {
        string s = ConfigurationManager.AppSettings["DbServerType"].ToString();
        string str1 = ConfigurationManager.AppSettings["Server"].ToString();
        string str2 = ConfigurationManager.AppSettings["CompanyDB"].ToString();
        string str3 = ConfigurationManager.AppSettings["DbUserName"].ToString();
        string str4 = ConfigurationManager.AppSettings["DbPassword"].ToString();
        string str5 = ConfigurationManager.AppSettings["UserName"].ToString();
        string str6 = ConfigurationManager.AppSettings["Password"].ToString();
        string str7 = ConfigurationManager.AppSettings["LicenseServer"].ToString();
        SAP.oCompany.DbServerType = (BoDataServerTypes) int.Parse(s);
        SAP.oCompany.Server = str1;
        SAP.oCompany.CompanyDB = str2;
        SAP.oCompany.DbUserName = str3;
        SAP.oCompany.DbPassword = str4;
        SAP.oCompany.UserName = str5;
        SAP.oCompany.Password = str6;
        SAP.oCompany.LicenseServer = str7;
        SAP.oCompany.XMLAsString = true;
        if (SAP.oCompany.Connect() != 0)
        {
          string errorDescription = SAP.oCompany.GetLastErrorDescription();
          SAP.Log.ErrorFormat("Erro ao conectar com Sap Business One: {0}", (object) errorDescription);
          throw new Exception("Falha! Erro: Erro ao conectar no SAP B1 - Descricao B1: " + errorDescription);
        }
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public static void Disconnect()
    {
      try
      {
        if (SAP.oCompany == null)
          return;
        if (SAP.oCompany.Connected)
          SAP.oCompany.Disconnect();
        Marshal.ReleaseComObject((object) SAP.oCompany);
      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
    }

    public static void AddTabela(string NomeTB, string Desc, BoUTBTableType oTableType)
    {
      string errMsg = "";
      UserTablesMD businessObject = (UserTablesMD) SAP.oCompany.GetBusinessObject(BoObjectTypes.oUserTables);
      try
      {
        businessObject.TableName = NomeTB.Replace("@", "").Replace("[", "").Replace("]", "").Trim();
        businessObject.TableDescription = Desc;
        businessObject.TableType = oTableType;
        try
        {
          businessObject.Add();
          int errCode;
          SAP.oCompany.GetLastError(out errCode, out errMsg);
          if (errCode != 0)
            throw new Exception(errMsg);
          Marshal.ReleaseComObject((object) businessObject);
          GC.Collect();
          GC.WaitForPendingFinalizers();
        }
        catch (Exception ex)
        {
          throw;
        }
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static void AddCampos(
      string NomeTabela,
      string NomeCampo,
      string DescCampo,
      BoFieldTypes Tipo,
      BoFldSubTypes SubTipo,
      short Tamanho,
      string[,] valoresValidos,
      string valorDefault,
      string linkedTable)
    {
      string errMsg = "";
      try
      {
        if (SAP.ExecuteSqlScalar("SELECT [name] FROM syscolumns WHERE [name] = 'U_" + NomeCampo + " ' and id = (SELECT id FROM sysobjects WHERE type = 'U'AND [NAME] = '" + NomeTabela.Replace("[", "").Replace("]", "") + "')") != null)
          return;
        UserFieldsMD businessObject = (UserFieldsMD) SAP.oCompany.GetBusinessObject(BoObjectTypes.oUserFields);
        businessObject.TableName = NomeTabela.Replace("@", "").Replace("[", "").Replace("]", "").Trim();
        businessObject.Name = NomeCampo;
        businessObject.Description = DescCampo;
        businessObject.Type = Tipo;
        businessObject.SubType = SubTipo;
        businessObject.DefaultValue = valorDefault;
        if (valoresValidos != null)
        {
          int length = valoresValidos.GetLength(0);
          if (length > 0)
          {
            for (int index = 0; index < length; ++index)
            {
              businessObject.ValidValues.Value = valoresValidos[index, 0];
              businessObject.ValidValues.Description = valoresValidos[index, 1];
              businessObject.ValidValues.Add();
            }
          }
        }
        if (Tamanho != (short) 0)
          businessObject.EditSize = (int) Tamanho;
        UserFieldsMD userFieldsMd;
        try
        {
          businessObject.Add();
          GC.Collect();
          GC.WaitForPendingFinalizers();
          GC.Collect();
          Marshal.ReleaseComObject((object) businessObject);
          userFieldsMd = (UserFieldsMD) null;
          int errCode;
          SAP.oCompany.GetLastError(out errCode, out errMsg);
          if (errCode != 0)
            throw new Exception(errMsg);
        }
        catch (Exception ex)
        {
          throw;
        }
        userFieldsMd = (UserFieldsMD) null;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public static bool ExisteTB(string TBName)
    {
      UserTablesMD businessObject = (UserTablesMD) SAP.oCompany.GetBusinessObject(BoObjectTypes.oUserTables);
      bool byKey = businessObject.GetByKey(TBName);
      string errMsg;
      SAP.oCompany.GetLastError(out int _, out errMsg);
      TBName = (string) null;
      errMsg = (string) null;
      Marshal.ReleaseComObject((object) businessObject);
      GC.Collect();
      GC.WaitForPendingFinalizers();
      GC.Collect();
      return byKey;
    }

    public static object ExecuteSqlScalar(string Query)
    {
      try
      {
        object obj = (object) null;
        Recordset businessObject = (Recordset) SAP.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
        businessObject.DoQuery(Query);
        if (!businessObject.EoF)
          obj = businessObject.Fields.Item((object) 0).Value;
        Marshal.ReleaseComObject((object) businessObject);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        return obj;
      }
      catch (Exception ex)
      {
        throw;
      }
    }
  }
}
