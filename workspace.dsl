workspace {

    // !identifiers hierarchical

    model {
        customer = person "Customer"

        notificationApp = softwareSystem "notificationApp" {
            tags "Internal"
            userService = container "User Service"
            paymentService = container "Payment Service"
            notificationService = container "Notification Service"
            messageBroker = container "Message Broker" {
                tags "External"
            }
        }
        NotificationProvider1 = softwareSystem "Notification Provider 1" {
                tags "External"
        }

        NotificationProvider2 = softwareSystem "Notification Provider 2" {
                tags "External"
        }

        notificationService -> NotificationProvider1 "sends to" "HTTP"
        notificationService -> NotificationProvider2 "sends to" "HTTP"

        userService -> messageBroker "publish events to" "AMQP"
        paymentService -> messageBroker "publish events to" "AMQP"

        notificationService ->  messageBroker "consumes from" "AMQP"


        NotificationProvider1 -> customer "sends to"
        NotificationProvider2 -> customer "sends to"


        customer -> notificationApp "uses"
    }


    views {
        systemContext notificationApp "Diagram1" {
            include *
            // autolayout lr
        }

        container notificationApp "Diagram2" {
            include *
            // autolayout lr
        }

        styles {
            element "Internal" {
                background #489C61 
            }
            element "Container" {
                background #60BD7C 
            }
            element "Person" {
                shape person
                background #418054 
            }
            element "Component" {
                background #91F0AE 
            }
            element "Database" {
                shape cylinder
            }
            element "External" {
                background silver
            }
            element "Browser" {
                shape WebBrowser
            }
        }
    }

    configuration {
        scope softwaresystem
    }

}