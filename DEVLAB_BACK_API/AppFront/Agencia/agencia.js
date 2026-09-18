const URL_API = 'http://localhost:5139/api/v1/Documento';


async function enviarDocumento() {
    const codigoCliente = document.getElementById("codigoCliente").value;
    const inputArquivo = document.getElementById("arquivo");
    const arquivo = inputArquivo.files[0];

    if (!codigoCliente || !arquivo) {
        alert("Informe o codigo do cliente e selecione um arquivo")
        return;
    }

    const dadosArquivo = new FormData();
    dadosArquivo.append("arquivo", arquivo);

    const response = await fetch(`${URL_API}/upload/${codigoCliente}`, {
        method: "POST",
        body: dadosArquivo
    });

    if (response.ok) {
        alert("Documento enviado com sucesso!");

        document.getElementById("codigocliente").value = "";
        document.getElementById("arquivo").value = "";


        document.getElementById("codigoBusca").value = codigoCliente;
        buscarDocumentos();
    }
    else {
        const erro = await response.json();
        alert("Erro " + (erro.messagem || "falha ao enviar o documento"));
    }
}



async function buscarDocumentos() {

    const codigoCliente = document.getElementById("codigoBusca").value;

    if (!codigoCliente) {
        alert("Informe o código do cliente.");
        return;
    }

    try {

        const response = await fetch(`${URL_API}/${codigoCliente}`);

        if (!response.ok) {
            alert("Não foi possível buscar os documentos.");
            return;
        }

        const documentos = await response.json();

        const lista = document.getElementById("listaDocumentos");

        lista.innerHTML = "";

        documentos.forEach(documento => {

            const linha = document.createElement("tr");

            linha.innerHTML = `
                <td>${documento.id}</td>
                <td>${documento.nome}</td>
                <td>${documento.extensao}</td>

                <td>

                    <button 
                        class="btn-baixar"
                        onclick="baixarDocumento(${documento.id}, '${codigoCliente}')">
                        Baixar
                    </button>

                    <button 
                        class="btn-excluir"
                        onclick="excluirDocumento(${documento.id}, '${codigoCliente}')">
                        Excluir
                    </button>

                </td>
            `;

            lista.appendChild(linha);

        });

    }
    catch (erro) {

        console.error(erro);

        alert("Erro ao buscar documentos.");

    }
}

 

async function baixarDocumento(id, codigoCliente) {

    try {

        const response = await fetch(
            `${URL_API}/download/${codigoCliente}/${id}`
        );

        if (!response.ok) {

            alert("Não foi possível baixar o arquivo.");

            return;
        }

        const blob = await response.blob();

        const url = window.URL.createObjectURL(blob);

        const link = document.createElement("a");

        link.href = url;

        link.download = `documento-${id}`;

        document.body.appendChild(link);

        link.click();

        link.remove();

        window.URL.revokeObjectURL(url);

    }
    catch (erro) {

        console.error(erro);

        alert("Erro ao baixar o arquivo.");

    }
}


async function excluirDocumento(id, codigoCliente) {

    const confirmar = confirm(
        "Tem certeza que deseja excluir este documento?"
    );

    if (!confirmar) {
        return;
    }

    try {

        const response = await fetch(
            `${URL_API}/${id}`,
            {
                method: "DELETE"
            }
        );

        if (!response.ok) {

            alert("Não foi possível excluir o documento.");

            return;
        }

        alert("Documento excluído com sucesso!");

      
        buscarDocumentos();

    }
    catch (erro) {

        console.error(erro);

        alert("Erro ao excluir o documento.");

    }
}