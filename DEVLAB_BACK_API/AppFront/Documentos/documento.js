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

        document.getElementById("codigoCliente").value = codigoCliente;
        document.getElementById("arquivo").value = arquivo;

        // Atualiza a tabela automaticamente
        document.getElementById("codigoBusca").value = codigoCliente;
        buscarDocumentos();
    }
    else {
        const erro = await response.json();

        alert(
            "Erro " +
            (erro.messagem || "falha ao enviar o documento")
        );
    }
}


/* BUSCAR DOCUMENTOS */

async function buscarDocumentos() {


    const response = await fetch(`${URL_API}/${codigoCliente}`);

    const documentos = await response.json();

    const lista = document.getElementById("listaDocumentos");

    lista.innerHTML = "";

    documentos.forEach(documento => {

        lista.innerHTML = `
            <td>${documento.id}</td>

            <td>${documento.nome}</td>

            <td>${documento.extensao}</td>

            <td>

                <button 
                    class="btn-baixar"
                    onclick="baixarDocumento(${documento.id}, ${codigoCliente})">
                    Baixar
                </button>

                <button 
                    class="btn-excluir"
                    onclick="excluirDocumento(${documento.id}, ${codigoCliente})">
                    Excluir
                </button>

            </td>
        `;
    });
}


/* DOWNLOAD */

async function baixarDocumento(id, codigoCliente) {

    const response = await fetch(
        `${URL_API}/download/${codigoCliente}/${id}`
    );

    if (!response.ok) {
        alert("Erro ao baixar documento.");
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


/* EXCLUSÃO */

async function excluirDocumento(id, codigoCliente) {

    const confirmar = confirm(
        "Deseja realmente excluir este documento?"
    );

    if (!confirmar) {
        return;
    }

    const response = await fetch(
        `${URL_API}/${id}`,
        {
            method: "DELETE"
        }
    );

    if (!response.ok) {
        alert("Erro ao excluir documento.");
        return;
    }

    alert("Documento excluído com sucesso!");

    // Atualiza a tabela
    buscarDocumentos();
}