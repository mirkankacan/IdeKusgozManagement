// Realtime Manager - Common SignalR functionality
class RealtimeManager {
    constructor() {
        this.connection = null;
        this.isConnected = false;
        this.connectionInfo = { userId: null, userName: null, roleName: null };
    }

    async initializeConnection(hubUrl, groups = []) {
        try {
            const token = await window.getAuthToken();

            this.connection = new signalR.HubConnectionBuilder()
                .withUrl(hubUrl, {
                    accessTokenFactory: () => token,
                    transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
                })
                .withAutomaticReconnect([0, 2000, 10000, 30000])
                .configureLogging(signalR.LogLevel.Error)
                .build();

            this.setupConnectionEvents();

            await this.connection.start();
            this.isConnected = true;

            // Join additional groups if needed
            for (const group of groups) {
                await this.connection.invoke("JoinGroup", group);
            }

            return true;
        } catch (error) {
            console.error('SignalR connection error:', error);
            this.showError('Bağlantı kurulamadı: ' + error.message, 'Bağlantı Hatası');
            return false;
        }
    }

    setupConnectionEvents() {
        this.connection.onclose((error) => {
            this.isConnected = false;
            this.connectionInfo = { userId: null, userName: null, roleName: null };
            if (error) {
                this.showError('Bağlantı kesildi, yeniden bağlanılıyor...', 'Bağlantı Hatası');
            }
        });

        this.connection.onreconnecting(() => {
            this.showWarning('Bağlantı yeniden kuruluyor...', 'Yeniden Bağlanıyor');
        });

        this.connection.onreconnected(() => {
            this.isConnected = true;
            this.showSuccess('Bağlantı başarıyla kuruldu', 'Bağlantı Başarılı');
        });

        this.connection.on("ConnectionStatus", (status) => {
            if (status.IsConnected) {
                this.isConnected = true;
                this.connectionInfo = {
                    userId: status.UserId,
                    userName: status.UserName,
                    roleName: status.RoleName
                };
            }
        });
    }

    showToast(message, title, type) {
        if (typeof toastr !== 'undefined') {
            toastr[type](message, title);
        }
    }

    showSuccess(message, title = 'Başarılı') { this.showToast(message, title, 'success'); }
    showError(message, title = 'Hata') { this.showToast(message, title, 'error'); }
    showWarning(message, title = 'Uyarı') { this.showToast(message, title, 'warning'); }
    showInfo(message, title = 'Bilgi') { this.showToast(message, title, 'info'); }

    playNotificationSound() {
        try {
            const audioContext = new (window.AudioContext || window.webkitAudioContext)();
            const oscillator = audioContext.createOscillator();
            const gainNode = audioContext.createGain();

            oscillator.connect(gainNode);
            gainNode.connect(audioContext.destination);

            oscillator.frequency.setValueAtTime(800, audioContext.currentTime);
            oscillator.frequency.setValueAtTime(600, audioContext.currentTime + 0.1);

            gainNode.gain.setValueAtTime(0.3, audioContext.currentTime);
            gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.3);

            oscillator.start();
            oscillator.stop(audioContext.currentTime + 0.3);
        } catch (error) {
            console.error('Could not play notification sound:', error);
        }
    }

    async joinGroup(groupName) {
        if (this.connection && this.isConnected) {
            await this.connection.invoke("JoinGroup", groupName);
        }
    }

    async leaveGroup(groupName) {
        if (this.connection && this.isConnected) {
            await this.connection.invoke("LeaveGroup", groupName);
        }
    }

    getConnectionInfo() { return this.connectionInfo; }
    getUserId() { return this.connectionInfo.userId; }
    getUserName() { return this.connectionInfo.userName; }
    getRoleName() { return this.connectionInfo.roleName; }

    on(eventName, callback) {
        if (this.connection) this.connection.on(eventName, callback);
    }

    off(eventName, callback) {
        if (this.connection) this.connection.off(eventName, callback);
    }

    async invoke(methodName, ...args) {
        if (this.connection && this.isConnected) {
            return await this.connection.invoke(methodName, ...args);
        }
    }

    disconnect() {
        if (this.connection) {
            this.connection.stop();
            this.connection = null;
            this.isConnected = false;
        }
    }
}

// Notification Manager
class NotificationManager extends RealtimeManager {
    constructor() {
        super();
        this.currentPage = 1;
        this.pageSize = 10;
        this.isLoading = false;
        this.hasMoreData = true;
        this.notifications = [];
        this.unreadCount = 0;
        this.init();
    }

    init() {
        this.loadUnreadCount();
        this.loadNotifications();
        this.setupEventListeners();
        this.setupSignalR();
    }

    setupEventListeners() {
        $('#notificationsDropdown').on('show.bs.dropdown', () => {
            this.renderAllNotifications();
            if (this.notifications.length === 0) this.loadNotifications();
        });

        $('#notificationsDropdown').on('click', e => e.stopPropagation());
        $('#loadMoreNotificationsBtn').on('click', () => this.loadMoreNotifications());
        $('#markAllAsReadBtn').on('click', e => { e.stopPropagation(); this.markAllAsRead(); });

        $(document).on('click', '.notification-item', (e) => {
            e.preventDefault();
            e.stopPropagation();

            const notificationId = $(e.currentTarget).data('notification-id');
            if (notificationId) this.markAsRead(notificationId);
            return false;
        });

        $('#notificationsContainer').on('scroll', () => {
            const container = document.getElementById('notificationsContainer');
            if (container.scrollTop + container.clientHeight >= container.scrollHeight - 10) {
                if (!this.isLoading && this.hasMoreData) {
                    this.loadMoreNotifications();
                }
            }
        });
    }

    async setupSignalR() {
        const success = await this.initializeConnection('http://localhost:5291/communicationHub');
        if (success) {
            this.on("NewNotification", notification => this.handleNewNotification(notification));
        }
    }

    async loadUnreadCount() {
        try {
            const response = await fetch('/bildirim/okunmayan-sayisi', {
                credentials: 'include',
                headers: { 'Content-Type': 'application/json' }
            });

            if (response.ok) {
                const result = await response.json();
                if (result.isSuccess) {
                    this.unreadCount = result.data;
                    this.updateUnreadCount();
                }
            }
        } catch (error) {
            console.error('Error loading unread count:', error);
        }
    }

    async loadNotifications(reset = true) {
        if (this.isLoading) return;

        try {
            this.isLoading = true;

            if (reset) {
                $('#notificationsList').empty();
                this.currentPage = 1;
                this.hasMoreData = true;
                this.notifications = [];
            }

            $('#loadingIndicator').show();

            const response = await fetch(`/bildirim?pageSize=${this.pageSize}&pageNumber=${this.currentPage}`, {
                credentials: 'include',
                headers: { 'Content-Type': 'application/json' }
            });

            if (response.ok) {
                const result = await response.json();
                if (result.isSuccess && result.data) {
                    const newNotifications = result.data.data || [];
                    this.notifications = reset ? newNotifications : [...this.notifications, ...newNotifications];
                    this.hasMoreData = newNotifications.length === this.pageSize;

                    this.sortNotifications();
                    this.renderNotifications(newNotifications, reset);

                    $('#loadMoreContainer')[this.hasMoreData ? 'show' : 'hide']();
                } else {
                    this.renderNotifications([], reset);
                }
            } else {
                this.renderNotifications([], reset);
            }
        } catch (error) {
            console.error('Error loading notifications:', error);
            this.showError('Bildirimler yüklenirken bir hata oluştu: ' + error.message);
            this.renderNotifications([], reset);
        } finally {
            this.isLoading = false;
            $('#loadingIndicator').hide();
        }
    }

    sortNotifications() {
        this.notifications.sort((a, b) => {
            if (a.isRead !== b.isRead) return a.isRead ? 1 : -1;
            return new Date(b.createdDate) - new Date(a.createdDate);
        });
    }

    async loadMoreNotifications() {
        this.currentPage++;
        await this.loadNotifications(false);
    }

    renderNotifications(notifications, reset) {
        const notificationsList = $('#notificationsList');

        if (reset) {
            notificationsList.empty();
            if (!notifications || notifications.length === 0) {
                notificationsList.html(this.getEmptyNotificationsHtml());
                return;
            }
        }

        notifications.forEach(notification => {
            const notificationHtml = this.createNotificationHtml(notification);
            notificationsList.append(notificationHtml);
            this.attachClickHandler(notification.id);
        });

        if (reset) this.renderAllNotifications();
    }

    renderAllNotifications() {
        const notificationsList = $('#notificationsList');
        notificationsList.empty();

        if (!this.notifications || this.notifications.length === 0) {
            notificationsList.html(this.getEmptyNotificationsHtml());
            return;
        }

        this.notifications.forEach(notification => {
            const notificationHtml = this.createNotificationHtml(notification);
            notificationsList.append(notificationHtml);
            this.attachClickHandler(notification.id);
        });
    }

    createNotificationHtml(notification) {
        const isUnread = !notification.isRead;
        const notificationClass = isUnread ? 'notification-item bold-notif-text' : 'notification-item';
        const formattedDate = new Date(notification.createdDate).toLocaleDateString('tr-TR', {
            day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit'
        });

        return `
            <div class="dropdown-item ${notificationClass}" data-notification-id="${notification.id}" data-notification-redirecturl="${notification.redirectUrl}" style="cursor: pointer;">
                <div class="d-flex align-items-start">
                    <div class="me-3">
                        <i class="fas fa-bell text-primary"></i>
                    </div>
                    <div class="flex-grow-1">
                        <p class="mb-1">${this.escapeHtml(notification.message)}</p>
                        <small class="text-muted">${formattedDate}</small>
                    </div>
                    ${!notification.isRead ? '<div class="ms-2"><span class="badge bg-primary rounded-pill" style="width: 8px; height: 8px;"></span></div>' : ''}
                </div>
            </div>
        `;
    }

    attachClickHandler(notificationId) {
        const element = $(`[data-notification-id="${notificationId}"]`);
        element.off('click').on('click', async (e) => {
            e.preventDefault();
            e.stopPropagation();

            const notification = this.notifications.find(n => n.id === notificationId);
            const redirectUrl = $(e.currentTarget).data('notification-redirecturl');

            if (notification && !notification.isRead) {
                // Okundu işareti başarılı olursa yönlendir
                const success = await this.markAsRead(notificationId);
                if (success && redirectUrl && redirectUrl.trim() !== '') {
                    setTimeout(() => {
                        window.location.href = redirectUrl;
                    }, 150);
                }
            } else {
                // Zaten okunmuşsa direkt yönlendir
                if (redirectUrl && redirectUrl.trim() !== '') {
                    setTimeout(() => {
                        window.location.href = redirectUrl;
                    }, 150);
                }
            }

            return false;
        });
    }

    getEmptyNotificationsHtml() {
        return `
            <div class="text-center py-4">
                <div class="mb-3">
                    <i class="fas fa-bell-slash text-muted" style="font-size: 2rem; opacity: 0.5;"></i>
                </div>
                <p class="text-muted mb-0 fw-normal">Henüz bildirim bulunmuyor</p>
                <small class="text-muted">Yeni bildirimler burada görünecek</small>
            </div>
        `;
    }

    async markAsRead(notificationId) {
        try {
            const response = await fetch(`/bildirim/${notificationId}/okundu`, {
                method: 'PUT',
                credentials: 'include',
                headers: { 'Content-Type': 'application/json' }
            });
            const result = await response.json();

            if (response.ok) {
                if (result.isSuccess) {
                    const notification = this.notifications.find(n => n.id === notificationId);
                    if (notification && !notification.isRead) {
                        notification.isRead = true;
                        notification.readDate = new Date().toISOString();
                        this.unreadCount = Math.max(0, this.unreadCount - 1);
                        this.updateUnreadCount();
                        this.renderAllNotifications();
                    }
                    return true; // Başarılı
                } else {
                    this.showError(result.message || 'Bildirim okundu olarak işaretlenemedi');
                    return false;
                }
            } else {
                this.showError(result.message);
                return false;
            }
        } catch (error) {
            console.error('Error marking notification as read:', error);
            this.showError('Bildirim okundu olarak işaretlenirken bir hata oluştu: ' + error.message);
            return false;
        }
    }

    async markAllAsRead() {
        try {
            const response = await fetch('/bildirim/hepsini-okundu', {
                method: 'PUT',
                credentials: 'include',
                headers: { 'Content-Type': 'application/json' }
            });

            if (response.ok) {
                this.notifications.forEach(notification => {
                    if (!notification.isRead) {
                        notification.isRead = true;
                        notification.readDate = new Date().toISOString();
                    }
                });
                this.unreadCount = 0;
                this.updateUnreadCount();
                this.renderAllNotifications();
                this.showSuccess('Tüm bildirimler okundu olarak işaretlendi');
            }
        } catch (error) {
            console.error('Error marking all notifications as read:', error);
            this.showError('Bildirimler işaretlenirken bir hata oluştu: ' + error.message);
        }
    }

    handleNewNotification(notification) {
        if (!this.shouldShowNotification(notification)) return;

        this.notifications.unshift(notification);
        this.unreadCount++;
        this.updateUnreadCount();
        this.renderAllNotifications();
        this.showToastNotification(notification);
    }

    shouldShowNotification(notification) {
        const currentUserRole = this.getRoleName();
        const currentUserId = this.getUserId();

        const targetRole = notification.targetRole || notification.TargetRole;
        const targetUserId = notification.targetUserId || notification.TargetUserId;

        if (!targetRole && !targetUserId) return true;
        if (targetUserId && targetUserId === currentUserId) return true;
        if (targetRole) {
            const targetRoles = targetRole.split(';');
            return targetRoles.includes(currentUserRole);
        }

        return false;
    }

    showToastNotification(notification) {
        this.showInfo(notification.message, 'Yeni Bildirim');
        this.playNotificationSound();
    }

    updateUnreadCount() {
        $('#notificationCount').text(this.unreadCount).show();
        $('#markAllAsReadBtn')[this.unreadCount > 0 ? 'show' : 'hide']();
    }

    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }
}

// Message Manager
class MessageManager extends RealtimeManager {
    constructor() {
        super();
        this.currentPage = 1;
        this.pageSize = 10;
        this.isLoading = false;
        this.hasMoreData = true;
        this.messages = [];
        this.init();
    }

    init() {
        this.loadMessages();
        this.setupEventListeners();
        this.setupSignalR();
    }

    setupEventListeners() {
        $('#sendMessageBtn').on('click', () => this.sendMessage());
        $('#messageInput').on('keypress', e => { if (e.key === 'Enter') this.sendMessage(); });
        $('#loadMoreMessagesBtn').on('click', () => this.loadMoreMessages());

        $('#messagesContainer').on('scroll', () => {
            const container = document.getElementById('messagesContainer');
            if (container.scrollTop + container.clientHeight >= container.scrollHeight - 10) {
                if (!this.isLoading && this.hasMoreData) {
                    this.loadMoreMessages();
                }
            }
        });
    }

    async setupSignalR() {
        const success = await this.initializeConnection('http://localhost:5291/communicationHub');
        if (success) {
            this.on("NewMessage", message => {
                console.log('Received NewMessage via SignalR:', message);
                this.handleNewMessage(message);
            });
            console.log('SignalR connection established for messages');
        } else {
            console.error('Failed to establish SignalR connection for messages');
        }
    }

    async loadMessages(reset = true) {
        if (this.isLoading) return;

        try {
            this.isLoading = true;
            $('#loadingIndicator').show();
            $('#errorMessage').hide();

            if (reset) {
                $('#messagesContainer').empty();
                this.currentPage = 1;
                this.hasMoreData = true;
                this.messages = [];
            }

            const response = await fetch(`/mesaj?pageSize=${this.pageSize}&pageNumber=${this.currentPage}`, {
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                const result = await response.json();
                if (result.isSuccess && result.data) {
                    const newMessages = result.data.data || [];
                    this.messages = reset ? newMessages : [...this.messages, ...newMessages];
                    this.hasMoreData = newMessages.length === this.pageSize;

                    this.messages.sort((a, b) => new Date(b.createdDate) - new Date(a.createdDate));
                    this.displayMessages(newMessages, reset);
                    $('#loadMoreContainer')[this.hasMoreData ? 'show' : 'hide']();
                } else {
                    throw new Error(result.message || 'Mesajlar yüklenirken bir hata oluştu');
                }
            } else {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
        } catch (error) {
            console.error('Error loading messages:', error);
            this.showError('Mesajlar yüklenirken bir hata oluştu: ' + error.message);
        } finally {
            this.isLoading = false;
            $('#loadingIndicator').hide();
        }
    }

    async loadMoreMessages() {
        this.currentPage++;
        await this.loadMessages(false);
    }

    displayMessages(newMessages, reset = true) {
        const messagesContainer = $('#messagesContainer');

        if (reset) {
            messagesContainer.empty();
            if (!newMessages || newMessages.length === 0) {
                messagesContainer.html('<div class="text-center text-muted py-3">Henüz mesaj bulunmuyor.</div>');
                return;
            }
            this.renderAllMessages();
            return;
        }

        newMessages.forEach(message => {
            messagesContainer.append(this.createMessageHtml(message));
        });
    }

    renderAllMessages() {
        const messagesContainer = $('#messagesContainer');
        messagesContainer.empty();

        if (!this.messages || this.messages.length === 0) {
            messagesContainer.html('<div class="text-center text-muted py-3">Henüz mesaj bulunmuyor.</div>');
            return;
        }

        this.messages.forEach(message => {
            console.log(message)
            messagesContainer.append(this.createMessageHtml(message));
        });
    }

    createMessageHtml(message) {
        const formattedDate = new Date(message.createdDate).toLocaleDateString('tr-TR', {
            day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit'
        });
        const isAll = !message.targetRoles && !message.targetUsers; 
        let targets;
        if (isAll) {
            targets = `
                        <small class="comment-target-roles">Görebilen Roller: Herkes</small>
                        <br/>
                        <small class="comment-target-users">Görebilen Kullanıcılar: Herkes</small>`;
        }
        else {
            if (message.targetRoles) {
                targets = `
                          <small class="comment-target-roles">Görebilen Roller: ${message.targetRoles}</small>`;
            }
            else if (message.targetUsers) {
                targets = `
                          <small class="comment-target-roles">Görebilen Kullanıcılar: ${message.targetUsers}</small>`;
            }
            else {
                targets = `
                        <small class="comment-target-roles">Görebilen Roller: ${message.targetRoles}</small>
                        <br/>
                        <small class="comment-target-users">Görebilen Kullanıcılar: ${message.targetUsers}</small>`;
            }
        }
        return `
            <div class="post-comm" data-message-id="${message.id}">
                <img src="/theme/assets/images/avatars/profile-image.png" class="comment-img" alt="">
                <div class="comment-container">
                    <span class="comment-author">
                        ${message.createdByFullName || message.createdBy}
                        <small class="comment-date">${formattedDate}</small>
                        <br/>
                        ${targets}
                    </span>
                </div>
                <span class="comment-text">${message.content}</span>
            </div>
        `;
    }

    handleNewMessage(message) {
        // Mesaj zaten varsa tekrar ekleme
        if (this.messages.find(m => m.id === message.id)) {
            console.log('Message already exists, skipping:', message.id);
            return;
        }

        console.log('Adding new message to UI:', message);
        this.messages.unshift(message);
        this.messages.sort((a, b) => new Date(b.createdDate) - new Date(a.createdDate));
        this.renderAllMessages();

        // Add animation
        const newMessageElement = $(`[data-message-id="${message.id}"]`);
        if (newMessageElement.length > 0) {
            newMessageElement.css({ opacity: '0', transform: 'translateY(-10px)', transition: 'all 0.3s ease' });
            setTimeout(() => newMessageElement.css({ opacity: '1', transform: 'translateY(0)' }), 100);
        }
    }

    async sendMessage() {
        const messageInput = $('#messageInput');
        const roleSelect = $('#select-target-roles');
        const userSelect = $('#select-target-users');

        const targetRoles = roleSelect.val() || []; // Boşsa boş array
        const targetUsers = userSelect.val() || []; // Boşsa boş array
        const messageContent = messageInput.val().trim();

        if (!messageContent) {
            this.showWarning('Lütfen bir mesaj yazın');
            return;
        }

        try {
            console.log('Sending message:', { content: messageContent, targetRoles, targetUsers });
            const response = await fetch('/mesaj', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
                },
                body: JSON.stringify({
                    content: messageContent,
                    targetRoles: targetRoles,     
                    targetUsers: targetUsers     
                })
            });

            if (response.ok) {
                const result = await response.json();
                console.log('Message send response:', result);
                if (result.isSuccess) {
                    // Mesajı hemen UI'ya ekle (gönderen kişi için)
                    if (result.data) {
                        console.log('Adding message to UI immediately:', result.data);
                        this.handleNewMessage(result.data);
                    } else {
                        console.warn('No message data in response');
                    }
                    
                    messageInput.val('');
                    roleSelect.val(null).trigger('change');
                    userSelect.val(null).trigger('change');
                    this.showSuccess('Mesaj başarıyla gönderildi');
                } else {
                    throw new Error(result.message || 'Mesaj gönderilirken bir hata oluştu');
                }
            } else {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
        } catch (error) {
            console.error('Error sending message:', error);
            this.showError('Mesaj gönderilirken bir hata oluştu: ' + error.message);
        }
    }
}

// Global auth token function
window.getAuthToken = async function () {
    try {
        const response = await fetch('/token-al', {
            credentials: 'include',
            headers: { 'Content-Type': 'application/json' }
        });

        if (response.ok) {
            const result = await response.json();
            return result.token || null;
        }
    } catch (error) {
        console.error('Failed to get token from session endpoint:', error);
    }
    console.warn('⚠️ No JWT token found for SignalR connection');
    return null;
};

// Initialize managers
$(document).ready(function () {
    if ($('#notificationsDropdown').length > 0) {
        window.notificationManager = new NotificationManager();
    }

    if ($('#messagesContainer').length > 0) {
        window.messageManager = new MessageManager();
    }
});