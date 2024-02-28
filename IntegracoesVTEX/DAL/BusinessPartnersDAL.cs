using System;
using System.Configuration;
using System.Runtime.InteropServices;
using IntegracoesVETX.Entity;
using IntegracoesVETX.Util;
using SAPbobsCOM;

namespace IntegracoesVETX.DAL
{
	public class BusinessPartnersDAL
	{
		private Company oCompany;

		private Log log;

		internal BusinessPartnersDAL()
		{
			log = new Log();
		}			

		public void InserirBusinessPartner(Company company, Cliente cliente, Pedido pedido,  out string messageError)
		{
			string document = string.Empty;
			bool isCorporate = false;
			bool marketPlace = false;

			try
			{
				CountyDAL countyDAL = new CountyDAL();
				oCompany = company;
				string cardCodePrefix = ConfigurationManager.AppSettings["CardCodePrefix"];
				document = pedido.clientProfileData.document;
				Log.WriteLogCliente("Inserindo Cliente " + cardCodePrefix + document);
				int _groupCode = Convert.ToInt32(ConfigurationManager.AppSettings["GroupCode"]);
				int _splCode = Convert.ToInt32(ConfigurationManager.AppSettings["SlpCode"]);
				Convert.ToInt32(ConfigurationManager.AppSettings["QoP"]);
				int groupNum = Convert.ToInt32(ConfigurationManager.AppSettings["GroupNum"]);
				string indicadorIE = ConfigurationManager.AppSettings["IndicadorIE"];
				string indicadorOpConsumidor = ConfigurationManager.AppSettings["IndicadorOpConsumidor"];
				string gerente = ConfigurationManager.AppSettings["Gerente"];
				int priceList = Convert.ToInt32(ConfigurationManager.AppSettings["PriceList"]);
				Convert.ToInt32(ConfigurationManager.AppSettings["CategoriaCliente"]);
				BusinessPartners oBusinessPartner = null;
				oBusinessPartner = (BusinessPartners)oCompany.GetBusinessObject(BoObjectTypes.oBusinessPartners);
				BusinessPartners oBusinessPartnerUpdateTest = null;
				oBusinessPartnerUpdateTest = (BusinessPartners)oCompany.GetBusinessObject(BoObjectTypes.oBusinessPartners);
				if (oBusinessPartnerUpdateTest.GetByKey(cardCodePrefix + document))
				{
					oBusinessPartner = oBusinessPartnerUpdateTest;
				}
				oBusinessPartner.CardCode = cardCodePrefix + document;
				oBusinessPartner.CardName = pedido.clientProfileData.firstName + " " + pedido.clientProfileData.lastName;
				oBusinessPartner.EmailAddress = cliente.email;
				oBusinessPartner.CardType = BoCardTypes.cCustomer;
				oBusinessPartner.GroupCode = _groupCode;
				oBusinessPartner.SalesPersonCode = _splCode;
				oBusinessPartner.PayTermsGrpCode = groupNum;
				oBusinessPartner.PriceListNum = priceList;
				oBusinessPartner.UserFields.Fields.Item("U_TX_IndIEDest").Value = indicadorIE;
				oBusinessPartner.UserFields.Fields.Item("U_TX_IndFinal").Value = indicadorOpConsumidor;
				oBusinessPartner.UserFields.Fields.Item("U_Gerente").Value = gerente;
				oBusinessPartner.UserFields.Fields.Item("U_CategoriaCliente").Value = gerente;
				oBusinessPartner.Phone1 = pedido.clientProfileData.phone.Substring(3);
				string codMunicipio = string.Empty;
				if (pedido.shippingData.address.city != null)
				{
					codMunicipio = countyDAL.RecuperarCodigoMunicipio(pedido.shippingData.address.city, oCompany);
				}
				oBusinessPartner.Addresses.SetCurrentLine(0);
				oBusinessPartner.Addresses.AddressType = BoAddressType.bo_BillTo;
				oBusinessPartner.Addresses.AddressName = "COBRANCA";
				oBusinessPartner.Addresses.City = pedido.shippingData.address.city;

				if (pedido.shippingData.address.complement != null && pedido.shippingData.address.complement.Length <= 100)
				{
					oBusinessPartner.Addresses.BuildingFloorRoom = pedido.shippingData.address.complement;
				}
				oBusinessPartner.Addresses.Block = pedido.shippingData.address.neighborhood;
				oBusinessPartner.Addresses.StreetNo = pedido.shippingData.address.number;
				oBusinessPartner.Addresses.ZipCode = pedido.shippingData.address.postalCode;
				oBusinessPartner.Addresses.State = pedido.shippingData.address.state;
				oBusinessPartner.Addresses.Street = pedido.shippingData.address.street;
				oBusinessPartner.Addresses.County = codMunicipio;

				oBusinessPartner.Addresses.Add();
				oBusinessPartner.Addresses.SetCurrentLine(1);
				oBusinessPartner.Addresses.AddressType = BoAddressType.bo_ShipTo;
				oBusinessPartner.Addresses.AddressName = "FATURAMENTO";
				oBusinessPartner.Addresses.City = pedido.shippingData.address.city;

				if (pedido.shippingData.address.complement != null && pedido.shippingData.address.complement.Length <= 100)
				{
					oBusinessPartner.Addresses.BuildingFloorRoom = pedido.shippingData.address.complement;
				}
				oBusinessPartner.Addresses.Block = pedido.shippingData.address.neighborhood;
				oBusinessPartner.Addresses.StreetNo = pedido.shippingData.address.number;
				oBusinessPartner.Addresses.ZipCode = pedido.shippingData.address.postalCode;
				oBusinessPartner.Addresses.State = pedido.shippingData.address.state;
				oBusinessPartner.Addresses.Street = pedido.shippingData.address.street;
				oBusinessPartner.Addresses.County = codMunicipio;

				oBusinessPartner.Addresses.Add();
				oBusinessPartner.BilltoDefault = "COBRANCA";
				oBusinessPartner.ShipToDefault = "FATURAMENTO";
				BusinessPartners oBusinessPartnerUpdate = null;
				oBusinessPartnerUpdate = (BusinessPartners)oCompany.GetBusinessObject(BoObjectTypes.oBusinessPartners);
				if (oBusinessPartnerUpdate.GetByKey(cardCodePrefix + document))
				{
					if (oBusinessPartner.Update() != 0)
					{
						messageError = oCompany.GetLastErrorDescription();
						log.WriteLogTable(oCompany, EnumTipoIntegracao.Cliente, document, cardCodePrefix + document, EnumStatusIntegracao.Erro, messageError);
					}
					else
					{
						messageError = "";
						log.WriteLogTable(oCompany, EnumTipoIntegracao.Cliente, document, cardCodePrefix + document, EnumStatusIntegracao.Sucesso, "Cliente atualizado com sucesso.");
						Marshal.ReleaseComObject(oBusinessPartner);
						Marshal.ReleaseComObject(oBusinessPartnerUpdate);
						Marshal.ReleaseComObject(oBusinessPartnerUpdateTest);
					}
				}
				else
				{
					oBusinessPartner.FiscalTaxID.TaxId4 = document;
					oBusinessPartner.FiscalTaxID.TaxId1 = "Isento";

					if (oBusinessPartner.Add() != 0)
					{
						messageError = oCompany.GetLastErrorDescription();
						log.WriteLogTable(oCompany, EnumTipoIntegracao.Cliente, document, "", EnumStatusIntegracao.Erro, messageError);
					}
					else
					{
						string CardCode = oCompany.GetNewObjectKey();
						log.WriteLogTable(oCompany, EnumTipoIntegracao.Cliente, document, CardCode, EnumStatusIntegracao.Sucesso, "Cliente inserido com sucesso.");
						messageError = "";
					}
				}
				Marshal.ReleaseComObject(oBusinessPartner);
				Marshal.ReleaseComObject(oBusinessPartnerUpdateTest);
				Marshal.ReleaseComObject(oBusinessPartnerUpdate);
			}
			catch (Exception e)
			{
				log.WriteLogTable(oCompany, EnumTipoIntegracao.Cliente, document, "", EnumStatusIntegracao.Erro, e.Message);
				Log.WriteLogCliente("InserirBusinessPartner Exception: " + e.Message);
				throw;
			}
		}

	}
}
