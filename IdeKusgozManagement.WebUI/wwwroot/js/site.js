// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Notification Management
class NotificationManager {
    constructor() {
        this.currentPage = 1;
        this.pageSize = 10;
        this.isLoading = false;
        this.hasMoreData = true;
        this.notifications = [];
        this.unreadCount = 0;
        this.connection = null;
        
        this.init();
    }

    init() {
        this.loadUnreadCount();
        this.loadNotifications();
        this.setupEventListeners();
        this.setupSignalR();
    }

    setupEventListeners() {
        // Dropdown show event
        $('#notificationsDropDown').on('show.bs.dropdown', () => {
            this.loadNotifications(true);
        });

        // Load more button
        $('#loadMoreBtn').on('click', () => {
            this.loadMoreNotifications();
        });

        // Mark all as read button
        $('#markAllAsReadBtn').on('click', () => {
            this.markAllAsRead();
        });

        // Individual notification click
        $(document).on('click', '.notification-item', (e) => {
            e.preventDefault();
            e.stopPropagation(); // Prevent dropdown from closing
            const notificationId = $(e.currentTarget).data('notification-id');
            this.markAsRead(notificationId);
        });

        // Infinite scroll
        $('#notificationsContainer').on('scroll', () => {
            const container = $('#notificationsContainer')[0];
            if (container.scrollTop + container.clientHeight >= container.scrollHeight - 10) {
                if (!this.isLoading && this.hasMoreData) {
                    this.loadMoreNotifications();
                }
            }
        });
    }

    setupSignalR() {
        // Get API base URL from configuration or use default
        const apiBaseUrl = window.location.origin.replace('localhost:5000', 'localhost:5001') || 'https://localhost:5001';
        
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(`${apiBaseUrl}/messageHub`)
            .withAutomaticReconnect()
            .build();

        this.connection.start().then(() => {
            console.log('SignalR Connected');
            // Join the Messages group to receive notifications
            this.connection.invoke("JoinGroup", "Messages");
        }).catch((err) => {
            console.error('SignalR Connection Error:', err);
        });

        // Listen for new notifications
        this.connection.on("NewNotification", (notification) => {
            console.log('New notification received:', notification);
            this.handleNewNotification(notification);
        });

        // Handle reconnection
        this.connection.onreconnecting(() => {
            console.log('SignalR Reconnecting...');
        });

        this.connection.onreconnected(() => {
            console.log('SignalR Reconnected');
            this.connection.invoke("JoinGroup", "Messages");
        });
    }

    handleNewNotification(notification) {
        // Add new notification to the beginning of the list
        this.notifications.unshift(notification);
        
        // Update unread count
        this.unreadCount++;
        $('#notificationCount').text(this.unreadCount);
        
        // If dropdown is open, refresh the display
        if ($('#notificationsDropdown').hasClass('show')) {
            this.renderAllNotifications();
        }
        
        // Show toast notification
        this.showToastNotification(notification);
    }

    renderAllNotifications() {
        const container = $('#notificationsList');
        container.empty();
        
        if (this.notifications.length === 0) {
            container.html(`
                <div class="text-center py-3">
                    <p class="text-muted mb-0">Henüz bildirim bulunmuyor</p>
                </div>
            `);
            return;
        }

        this.notifications.forEach(notification => {
            const notificationHtml = `
                <a href="#" class="notification-item" data-notification-id="${notification.id}">
                    <div class="header-notif">
                        <div class="notif-image">
                            <span class="notification-badge bg-info text-white">
                                <i class="fas fa-bell"></i>
                            </span>
                        </div>
                        <div class="notif-text">
                            <p class="${notification.isRead ? '' : 'bold-notif-text'}">${this.escapeHtml(notification.message)}</p>
                            <small>${this.formatDate(notification.createdDate)}</small>
                        </div>
                    </div>
                </a>
            `;
            container.append(notificationHtml);
        });
    }

    showToastNotification(notification) {
        // Show a toast notification for new notifications
        if (typeof toastr !== 'undefined') {
            toastr.info(notification.message, 'Yeni Bildirim', {
                timeOut: 5000,
                closeButton: true,
                progressBar: true
            });
        }
    }

    async loadUnreadCount() {
        try {
            const response = await fetch('/bildirim/okunmayan-sayisi');
            const data = await response.json();
            
            if (data.isSuccess) {
                this.unreadCount = data.data;
                $('#notificationCount').text(this.unreadCount);
            }
        } catch (error) {
            console.error('Error loading unread count:', error);
        }
    }

    async loadNotifications(reset = false) {
        if (this.isLoading) return;
        
        if (reset) {
            this.currentPage = 1;
            this.hasMoreData = true;
            this.notifications = [];
            $('#notificationsList').empty();
        }

        this.isLoading = true;
        $('#loadingIndicator').show();

        try {
            const response = await fetch(`/bildirim?pageSize=${this.pageSize}&pageNumber=${this.currentPage}`);
            const data = await response.json();
            
            if (data.isSuccess) {
                const newNotifications = data.data.data || [];
                this.notifications = reset ? newNotifications : [...this.notifications, ...newNotifications];
                
                // Sort notifications: unread first, then by creation date (newest first)
                this.notifications.sort((a, b) => {
                    if (a.isRead !== b.isRead) {
                        return a.isRead ? 1 : -1; // Unread first
                    }
                    return new Date(b.createdDate) - new Date(a.createdDate); // Newest first
                });
                
                this.hasMoreData = newNotifications.length === this.pageSize;
                this.renderNotifications(newNotifications, reset);
                
                // Show/hide load more button
                if (this.hasMoreData) {
                    $('#loadMoreContainer').show();
                } else {
                    $('#loadMoreContainer').hide();
                }

                // Show/hide mark all as read button
                if (this.notifications.some(n => !n.isRead)) {
                    $('#markAllAsReadBtn').show();
                } else {
                    $('#markAllAsReadBtn').hide();
                }
            }
        } catch (error) {
            console.error('Error loading notifications:', error);
        } finally {
            this.isLoading = false;
            $('#loadingIndicator').hide();
        }
    }

    async loadMoreNotifications() {
        this.currentPage++;
        await this.loadNotifications(false);
    }

    renderNotifications(notifications, reset = false) {
        if (reset) {
            this.renderAllNotifications();
        } else {
            // For pagination, append new notifications
            const container = $('#notificationsList');
            notifications.forEach(notification => {
                const notificationHtml = `
                    <a href="#" class="notification-item" data-notification-id="${notification.id}">
                        <div class="header-notif">
                            <div class="notif-image">
                                <span class="notification-badge bg-info text-white">
                                    <i class="fas fa-bell"></i>
                                </span>
                            </div>
                            <div class="notif-text">
                                <p class="${notification.isRead ? '' : 'bold-notif-text'}">${this.escapeHtml(notification.message)}</p>
                                <small>${this.formatDate(notification.createdDate)}</small>
                            </div>
                        </div>
                    </a>
                `;
                container.append(notificationHtml);
            });
        }
    }

    async markAsRead(notificationId) {
        try {
            const response = await fetch(`/bildirim/${notificationId}/okundu`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            
            const data = await response.json();
            
            if (data.isSuccess) {
                // Update local state
                const notification = this.notifications.find(n => n.id === notificationId);
                if (notification) {
                    notification.isRead = true;
                }
                
                // Update UI
                const notificationElement = $(`.notification-item[data-notification-id="${notificationId}"]`);
                notificationElement.find('p').removeClass('bold-notif-text');
                
                // Update unread count
                this.unreadCount = Math.max(0, this.unreadCount - 1);
                $('#notificationCount').text(this.unreadCount);
                
                // Hide mark all as read button if no unread notifications
                if (!this.notifications.some(n => !n.isRead)) {
                    $('#markAllAsReadBtn').hide();
                }
            }
        } catch (error) {
            console.error('Error marking notification as read:', error);
        }
    }

    async markAllAsRead() {
        try {
            const response = await fetch('/bildirim/hepsini-okundu', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            
            const data = await response.json();
            
            if (data.isSuccess) {
                // Update local state
                this.notifications.forEach(n => n.isRead = true);
                
                // Update UI
                $('.notification-item p').removeClass('bold-notif-text');
                
                // Update unread count
                this.unreadCount = 0;
                $('#notificationCount').text('0');
                
                // Hide mark all as read button
                $('#markAllAsReadBtn').hide();
            }
        } catch (error) {
            console.error('Error marking all notifications as read:', error);
        }
    }

    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    formatDate(dateString) {
        const date = new Date(dateString);
        return date.toLocaleDateString('tr-TR', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    }
}

// Initialize notification manager when document is ready
$(document).ready(function() {
    window.notificationManager = new NotificationManager();
});