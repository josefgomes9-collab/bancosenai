const URL_API = 'https://local:host:7081/api/v1/Documento';


async function enviarDocumento() {
    const codigoCliente = documento.getElementById("codigoCliente").value;
    alerta("o código do cliente é" + codigoCliente);
    const inputArquivo= documento.getElementById("arquivo");
    const arquivo = inputArquivo.files[0];

    if (!codigoCliente || arquivo) {
        alert("Informe o codigo do cliente e selecione um arquivo")
        return;
    }

    const dadosArquivo = new FormData();
    dadosArquivo.append("arquivo", arquivo);
    const response = await fetch(${ URL_API } / upload / ${ codigoCliente }, {
        method: "POST",
        body: dadosArquivo
    });

    if (response.ok) {
        alert("Documento enviado com sucesso!");
        document.getElementById("codigocliente").value = "";
        document.getElementById("arquivo").value = "";
    }
    else {
        const erro = await response.json();
        ("Erro" + (erro.messagem || "falha ao enviar o documento"));
    }
}
}