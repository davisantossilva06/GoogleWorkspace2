EnableDefaultContentItems = true
document.addEventListener("DOMContentLoaded", () => {
    inicializarFormularioLogin();
    inicializarFormularioCriarSenha();
});


function inicializarFormularioLogin() {
    const formulario = document.querySelector("form");
    if (!formulario) return;

    const campoEmail = document.querySelector("input[name='Email']");
    const campoSenha = document.querySelector("input[name='Senha']");
    const areaErro = document.querySelector(".erro");

    formulario.addEventListener("submit", () => {
        if (areaErro) areaErro.innerText = "";

        if (!campoEmail || !campoSenha) return;

        if (campoEmail.value.trim() === "") {
            mostrarErro(areaErro, "Informe o email institucional");
            event.preventDefault();
            return;
        }

        if (campoSenha.value.trim() === "") {
            mostrarErro(areaErro, "Informe a senha");
            event.preventDefault();
            return;
        }
    });
}

function inicializarFormularioCriarSenha() {
    const formulario = document.querySelector("form");
    if (!formulario) return;

    const campoNovaSenha = document.querySelector("input[name='NovaSenha']");
    const campoConfirmacao = document.querySelector("input[name='ConfirmacaoSenha']");
    const areaErro = document.querySelector(".erro");

    if (!campoNovaSenha || !campoConfirmacao) return;

    formulario.addEventListener("submit", (event) => {
        if (areaErro) areaErro.innerText = "";

        const senha = campoNovaSenha.value.trim();
        const confirmacao = campoConfirmacao.value.trim();

        if (senha.length < 8) {
            mostrarErro(areaErro, "A senha deve ter no mínimo 8 caracteres");
            event.preventDefault();
            return;
        }

        if (senha !== confirmacao) {
            mostrarErro(areaErro, "As senhas não coincidem");
            event.preventDefault();
            return;
        }
    });
}

function mostrarErro(elemento, mensagem) {
    if (!elemento) return;
    elemento.innerText = mensagem;
    elemento.style.display = "block";
}

function esconderErro(elemento) {
    if (!elemento) return;
    elemento.innerText = "";
    elemento.style.display = "none";
}

function autenticarGoogle(g_id_signin)
        if ()

