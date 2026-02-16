from flask import Flask, render_template, request, jsonify, redirect, url_for
from flask_cors import CORS

app = Flask(__name__)

CORS(app)

@app.route('/login')
def index():
    return render_template('login.html')

@app.route('/login', methods=['POST'])
def login():
    data = request.get_json()
    username = data.get('Username')
    password = data.get('Password')
    redirect_url = data.get('ReturnUrl') or url_for('index')

    print("Username : ", username)
    print("Password : ", password)
    return jsonify(success=True, redirectUrl=redirect_url)

if __name__ == "__main__":
    app.run(host="0.0.0.0")

