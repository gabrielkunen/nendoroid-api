# nendoroid-api

# Extração de dados (web crawler)

Esta é uma aplicação console utilizando .NET na versão 8.0, que extrai os dados das nendoroids do site da Good Smile Company, utilizei ela para popular o banco de dados Postgres com as informações das nendoroids e os links de suas imagens. Para a extração dos dados presentes no HTML, foi utilizado o pacote HtmlAgilityPack que possibilita extrair os dados utilizando o XPath.

Os erros encontrados durante a execução são armazenados no ./log/log.txt, é útil para quando as páginas das nendoroids fogem do padrão, como a nendoroid número 1538, que é especial e possui uma página diferenciada para ela, além de apresentar outros erros e não interferir na extração das informações das outras nendoroids.

Tecnologias utilizadas:

* .NET 8.0
* HtmlAgilityPack 1.11.54
* Dapper 2.1.24
* Npgsql 8.0.1

---