SELECT T0.DocNum AS docNPV ,T0.NumAtCard AS idOrderVtex , T0.U_NumPedEXT AS idOrderVtex2 
,T2.DocEntry AS externalId ,T2.DocNum AS docSAP ,T2.Serial AS invoiceNumber ,T2.DocDate AS invoiceDate 
,T3.KeyNfe AS nfeKey ,T0.PickRmrk AS shippingMethod ,T2.SeriesStr AS invoiceOrderSeries 
,T1.ItemCode AS codItem ,T1.Price AS precoItem ,T1.Quantity AS qtdItem ,T0.DocTotal AS totalNF 
FROM    ORDR T0 INNER JOIN INV1 T1 ON T0.DocEntry = T1.BaseEntry  
INNER JOIN OINV T2 ON T1.DocEntry = T2.DocEntry and T0.BPLId = T2.BPLId
INNER JOIN [SBO_TaxOne].[dbo].[Entidade] tax on tax.BusinessPlaceId = t0.BPLId
INNER JOIN [DBInvOne].[dbo].[Process] T3 on tax.ID = t3.CompanyId and T3.DocEntry = T2.DocEntry
WHERE T0.U_PLATF = '{0}'
AND  ISNULL (T2.NumAtCard,'') <> ''
AND ISNULL (T2.U_EnvioNFVTEX,'') = '' 
ORDER BY docNPV desc
