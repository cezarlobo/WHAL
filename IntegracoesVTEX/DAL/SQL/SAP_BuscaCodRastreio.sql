SELECT  DISTINCT  U.[U_CodRastreio] AS codigoRastreamento , R.CardCode AS cardCode , R.CardName AS nomeDestinatario , R.E_Mail AS emailDestinatario 
, T2.Serial AS invoiceNumber , T0.DocNum AS docNPV  , T0.NumAtCard AS idOrderVtex  , T0.U_NumPedEXT AS idOrderVtex2 , T2.DocEntry AS docEntry  
, T2.DocNum AS docNum 
FROM    ORDR T0  
INNER JOIN INV1 T1 ON T0.DocEntry = T1.BaseEntry   
INNER JOIN OINV T2 ON T1.DocEntry = T2.DocEntry and T0.BPLId = T2.BPLId   
INNER JOIN [SBO_TaxOne].[dbo].[Entidade] tax on tax.BusinessPlaceId = t0.BPLId
INNER JOIN [DBInvOne].[dbo].[Process] T3 on tax.ID = t3.CompanyId and T3.DocEntry = T2.DocEntry  
INNER JOIN OCRD R ON R.CardCode = T0.CardCode 
INNER JOIN [WahlClipper].[dbo].[@WAHL_COD_RASTREIO] U ON U.[U_NumNF] = T2.Serial 
WHERE T0.U_PLATF = '{0}' AND T2.U_EnvioNFVTEX = 'S' AND (T2.U_ValidaEnvioCodRastreio IS NULL OR T2.U_ValidaEnvioCodRastreio = '') 
AND T2.Serial IN (SELECT DISTINCT [U_NumNF] FROM [WahlClipper].[dbo].[@WAHL_COD_RASTREIO]) AND (R.E_Mail IS NOT NULL OR R.E_Mail <> '') AND YEAR (T2.TaxDate) >= '2024'
and ISNULL(T2.NumAtCard,'') <> ''
ORDER BY docNPV DESC  