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

		public void InserirBusinessPartner(Company company, Cliente cliente, Endereco endereco, Pedido pedido, out string messageError)
		{
			string document = string.Empty;
			bool isCorporate = false;
			bool marketPlace = false;
			if (pedido.origin.Equals("Fulfillment"))
			{
				marketPlace = true;
			}
			if (marketPlace)
			{
				document = pedido.clientProfileData.document;
			}
			if (cliente.isCorporate != null && cliente.isCorporate.Equals("true"))
			{
				document = cliente.corporateDocument;
				isCorporate = true;
			}
			else if (cliente.isCorporate != null && cliente.isCorporate.Equals("false"))
			{
				document = cliente.document;
			}
			try
			{
				CountyDAL countyDAL = new CountyDAL();
				oCompany = company;
				int _groupCode = Convert.ToInt32(ConfigurationManager.AppSettings["GroupCode"]);
				int _splCode = Convert.ToInt32(ConfigurationManager.AppSettings["SlpCode"]);
				Convert.ToInt32(ConfigurationManager.AppSettings["QoP"]);
				int groupNum = Convert.ToInt32(ConfigurationManager.AppSettings["GroupNum"]);
				string indicadorIE = ConfigurationManager.AppSettings["IndicadorIE"];
				string indicadorOpConsumidor = ConfigurationManager.AppSettings["IndicadorOpConsumidor"];
				string gerente = ConfigurationManager.AppSettings["Gerente"];
				int priceList = Convert.ToInt32(ConfigurationManager.AppSettings["PriceList"]);
				string cardCodePrefix = ConfigurationManager.AppSettings["CardCodePrefix"];
				Convert.ToInt32(ConfigurationManager.AppSettings["CategoriaCliente"]);
				Log.WriteLogCliente("Inserindo Cliente " + cardCodePrefix + document);
				BusinessPartners oBusinessPartner = null;
				oBusinessPartner = (BusinessPartners)oCompany.GetBusinessObject(BoObjectTypes.oBusinessPartners);
				BusinessPartners oBusinessPartnerUpdateTest = null;
				oBusinessPartnerUpdateTest = (BusinessPartners)oCompany.GetBusinessObject(BoObjectTypes.oBusinessPartners);
				if (oBusinessPartnerUpdateTest.GetByKey(cardCodePrefix + document))
				{
					oBusinessPartner = oBusinessPartnerUpdateTest;
				}
				oBusinessPartner.CardCode = cardCodePrefix + document;
				if (marketPlace)
				{
					oBusinessPartner.CardName = pedido.clientProfileData.firstName + " " + pedido.clientProfileData.lastName;
				}
				else
				{
					oBusinessPartner.CardName = cliente.firstName + " " + cliente.lastName;
					oBusinessPartner.EmailAddress = cliente.email;
				}
				oBusinessPartner.CardType = BoCardTypes.cCustomer;
				oBusinessPartner.GroupCode = _groupCode;
				oBusinessPartner.SalesPersonCode = _splCode;
				oBusinessPartner.PayTermsGrpCode = groupNum;
				oBusinessPartner.PriceListNum = priceList;
				oBusinessPartner.UserFields.Fields.Item("U_TX_IndIEDest").Value = indicadorIE;
				oBusinessPartner.UserFields.Fields.Item("U_TX_IndFinal").Value = indicadorOpConsumidor;
				oBusinessPartner.UserFields.Fields.Item("U_Gerente").Value = gerente;
				oBusinessPartner.UserFields.Fields.Item("U_CategoriaCliente").Value = gerente;
				if (cliente.homePhone != null)
				{
					oBusinessPartner.Phone1 = cliente.homePhone.Substring(2);
				}
				if (cliente.phone != null)
				{
					oBusinessPartner.Cellular = cliente.phone.Substring(2);
				}
				if (marketPlace)
				{
					oBusinessPartner.Phone1 = pedido.clientProfileData.phone;
				}
				string codMunicipio = string.Empty;
				if (!marketPlace)
				{
					codMunicipio = countyDAL.RecuperarCodigoMunicipio(endereco.city, oCompany);
				}
				else if (pedido.shippingData.address.city != null)
				{
					codMunicipio = countyDAL.RecuperarCodigoMunicipio(pedido.shippingData.address.city, oCompany);
				}
				oBusinessPartner.Addresses.SetCurrentLine(0);
				oBusinessPartner.Addresses.AddressType = BoAddressType.bo_BillTo;
				oBusinessPartner.Addresses.AddressName = "COBRANCA";
				if (marketPlace)
				{
					oBusinessPartner.Addresses.City = pedido.shippingData.address.city;
				}
				else
				{
					oBusinessPartner.Addresses.City = endereco.city;
				}
				if (marketPlace && pedido.shippingData.address.complement != null && pedido.shippingData.address.complement.Length <= 100)
				{
					oBusinessPartner.Addresses.BuildingFloorRoom = pedido.shippingData.address.complement;
				}
				else if (endereco != null && endereco.complement != null && endereco.complement.Length <= 100)
				{
					oBusinessPartner.Addresses.BuildingFloorRoom = endereco.complement;
				}
				if (marketPlace)
				{
					oBusinessPartner.Addresses.Block = pedido.shippingData.address.neighborhood;
					oBusinessPartner.Addresses.StreetNo = pedido.shippingData.address.number;
					oBusinessPartner.Addresses.ZipCode = pedido.shippingData.address.postalCode;
					oBusinessPartner.Addresses.State = pedido.shippingData.address.state;
					oBusinessPartner.Addresses.Street = pedido.shippingData.address.street;
					oBusinessPartner.Addresses.County = codMunicipio;
				}
				else
				{
					oBusinessPartner.Addresses.Block = endereco.neighborhood;
					oBusinessPartner.Addresses.StreetNo = endereco.number;
					oBusinessPartner.Addresses.ZipCode = endereco.postalCode;
					oBusinessPartner.Addresses.State = endereco.state;
					oBusinessPartner.Addresses.Street = endereco.street;
					oBusinessPartner.Addresses.County = codMunicipio;
				}
				oBusinessPartner.Addresses.Add();
				oBusinessPartner.Addresses.SetCurrentLine(1);
				oBusinessPartner.Addresses.AddressType = BoAddressType.bo_ShipTo;
				oBusinessPartner.Addresses.AddressName = "FATURAMENTO";
				if (marketPlace)
				{
					oBusinessPartner.Addresses.City = pedido.shippingData.address.city;
				}
				else
				{
					oBusinessPartner.Addresses.City = endereco.city;
				}
				if (marketPlace && pedido.shippingData.address.complement != null && pedido.shippingData.address.complement.Length <= 100)
				{
					oBusinessPartner.Addresses.BuildingFloorRoom = pedido.shippingData.address.complement;
				}
				else if (endereco != null && endereco.complement != null && endereco.complement.Length <= 100)
				{
					oBusinessPartner.Addresses.BuildingFloorRoom = endereco.complement;
				}
				if (marketPlace)
				{
					oBusinessPartner.Addresses.Block = pedido.shippingData.address.neighborhood;
					oBusinessPartner.Addresses.StreetNo = pedido.shippingData.address.number;
					oBusinessPartner.Addresses.ZipCode = pedido.shippingData.address.postalCode;
					oBusinessPartner.Addresses.State = pedido.shippingData.address.state;
					oBusinessPartner.Addresses.Street = pedido.shippingData.address.street;
					oBusinessPartner.Addresses.County = codMunicipio;
				}
				else
				{
					oBusinessPartner.Addresses.Block = endereco.neighborhood;
					oBusinessPartner.Addresses.StreetNo = endereco.number;
					oBusinessPartner.Addresses.ZipCode = endereco.postalCode;
					oBusinessPartner.Addresses.State = endereco.state;
					oBusinessPartner.Addresses.Street = endereco.street;
					oBusinessPartner.Addresses.County = codMunicipio;
				}
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
					if (isCorporate)
					{
						oBusinessPartner.FiscalTaxID.TaxId0 = document;
					}
					else
					{
						oBusinessPartner.FiscalTaxID.TaxId4 = document;
						oBusinessPartner.FiscalTaxID.TaxId1 = "Isento";
					}
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
