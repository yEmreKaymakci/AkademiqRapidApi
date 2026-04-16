const messageInput = document.getElementById('novaInput');
const chatBox = document.getElementById('novaMessages');
const typingIndicator = document.getElementById('novaTyping');
const sendBtn = document.getElementById('novaSendBtn');

const TYPING_SPEED = 15;

messageInput.addEventListener("keypress", function (event) {
    if (event.key === "Enter") {
        event.preventDefault();
        sendMessage();
    }
});

sendBtn.addEventListener("click", function(event) {
    event.preventDefault();
    sendMessage();
});

function formatTime() {
    const now = new Date();
    return now.getHours().toString().padStart(2, '0') + ':' + now.getMinutes().toString().padStart(2, '0');
}

async function sendMessage() {
    const messageText = messageInput.value.trim();
    if (!messageText) return;

    appendMessage(messageText, 'user');
    messageInput.value = '';

    typingIndicator.style.display = 'block';
    scrollToBottom();

    // The stream URL is handled by the data attribute on the container
    const streamUrl = document.querySelector('.nova-container').dataset.streamUrl;

    try {
        const response = await fetch(streamUrl, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ message: messageText })
        });

        typingIndicator.style.display = 'none';

        if (!response.ok) {
            appendMessage('Sistem Hatası: Bağlantı kurulamadı.', 'bot');
            return;
        }

        const botMessageDiv = createEmptyMessageBubble('bot');
        const reader = response.body.getReader();
        const decoder = new TextDecoder("utf-8");

        let textQueue = "";
        let isTyping = false;

        async function typeNextCharacter() {
            if (textQueue.length > 0) {
                isTyping = true;
                botMessageDiv.textContent += textQueue.charAt(0);
                textQueue = textQueue.substring(1);

                scrollToBottom();

                setTimeout(typeNextCharacter, TYPING_SPEED);
            } else {
                isTyping = false;
            }
        }

        while (true) {
            const { done, value } = await reader.read();
            if (done) break;

            const chunk = decoder.decode(value, { stream: true });
            textQueue += chunk;

            if (!isTyping) {
                typeNextCharacter();
            }
        }

    } catch (error) {
        typingIndicator.style.display = 'none';
        appendMessage('Sistem Hatası: Ağ bağlantısı koptu.', 'bot');
    }
}

function appendMessage(text, sender) {
    const messageDiv = createEmptyMessageBubble(sender);
    messageDiv.textContent = text;
    scrollToBottom();
}

function createEmptyMessageBubble(sender) {
    const wrapper = document.createElement('div');
    wrapper.classList.add('msg-wrapper', sender);

    const bubble = document.createElement('div');
    bubble.classList.add('msg-bubble');
    
    const info = document.createElement('div');
    info.classList.add('msg-info');
    info.textContent = sender === 'user' ? `OPERATOR_01 • ${formatTime()}` : `NOVA • ${formatTime()}`;

    wrapper.appendChild(bubble);
    wrapper.appendChild(info);

    chatBox.insertBefore(wrapper, typingIndicator);
    return bubble;
}

function scrollToBottom() {
    chatBox.scrollTop = chatBox.scrollHeight;
}
