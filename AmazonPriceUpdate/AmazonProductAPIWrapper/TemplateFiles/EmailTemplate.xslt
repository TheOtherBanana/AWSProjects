<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" indent="yes" />

  <xsl:template match="/*">
    <p>
      Hi you have some price updates for the following products.
    </p>

    <p>
      Please use the below link to purchase the products.
    </p>

    <table width="600" border="0" cellspacing="0" cellpadding="0" style="font-family: calibri,arial,helvetica,verdana,sans-serif;
										background-color: #ffffff; border: solid 1px #dddddd; -moz-border-radius: 4px;
										-webkit-border-radius: 4px; border-radius: 4px; line-height: normal;">
      <tbody>
        <tr>
          <th>Item Name</th>
          <th>Original Price</th>
          <th>Current Price</th>
          <th>Purchase Link</th>
        </tr>
        <xsl:for-each select="ProductsToEmail/ProductsToEmailItem">
          <tr>
            <td>
              <xsl:value-of select="ProductName" />
            </td>
            <td>
              <xsl:value-of select="CurrentPrice" />
            </td>
            <td>
              <xsl:value-of select="InitialPrice" />
            </td>
            <td>
              <xsl:value-of select="ProductPurchaseLink" />
            </td>
          </tr>
        </xsl:for-each>
      </tbody>
    </table>
  </xsl:template>
</xsl:stylesheet>