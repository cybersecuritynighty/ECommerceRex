import os
import json
import base64
import cv2
import numpy as np
import face_recognition
from flask import Flask, request, jsonify
from flask_cors import CORS

app = Flask(__name__)
CORS(app)  # Allow CORS for .NET app calls

# In‑memory cache of known face encodings (for speed)
# In production, you'd fetch from .NET DB, but we'll rely on .NET to pass embeddings.
# We'll just store a dict mapping user_id -> list of encodings.
# For simplicity, we assume the .NET app sends the user's stored encodings per request.

@app.route('/health', methods=['GET'])
def health():
    return jsonify({"status": "ok"})

@app.route('/register', methods=['POST'])
def register():
    """
    Expects a JSON with:
      - "image": base64-encoded image (data URL or raw base64)
      - "user_id": int
    Returns the face encoding as a list of floats.
    """
    data = request.json
    image_b64 = data.get('image')
    user_id = data.get('user_id')

    if not image_b64 or not user_id:
        return jsonify({"error": "Missing image or user_id"}), 400

    # Decode base64 to image
    if ',' in image_b64:
        image_b64 = image_b64.split(',')[1]  # remove data URL prefix
    img_data = base64.b64decode(image_b64)
    np_arr = np.frombuffer(img_data, np.uint8)
    img = cv2.imdecode(np_arr, cv2.IMREAD_COLOR)

    # Find face locations and encodings
    face_locations = face_recognition.face_locations(img)
    if not face_locations:
        return jsonify({"error": "No face detected"}), 400

    face_encodings = face_recognition.face_encodings(img, face_locations)
    if not face_encodings:
        return jsonify({"error": "Could not extract face encoding"}), 400

    # Use the first face
    encoding = face_encodings[0].tolist()
    return jsonify({"user_id": user_id, "encoding": encoding})

@app.route('/recognize', methods=['POST'])
def recognize():
    """
    Expects:
      - "image": base64-encoded image
      - "known_encodings": list of { "user_id": int, "encoding": [float] }
    Returns the best matching user_id and distance.
    """
    data = request.json
    image_b64 = data.get('image')
    known_encodings = data.get('known_encodings', [])

    if not image_b64:
        return jsonify({"error": "Missing image"}), 400
    if not known_encodings:
        return jsonify({"error": "No known encodings provided"}), 400

    # Decode image
    if ',' in image_b64:
        image_b64 = image_b64.split(',')[1]
    img_data = base64.b64decode(image_b64)
    np_arr = np.frombuffer(img_data, np.uint8)
    img = cv2.imdecode(np_arr, cv2.IMREAD_COLOR)

    # Detect faces in the image
    face_locations = face_recognition.face_locations(img)
    if not face_locations:
        return jsonify({"error": "No face detected"}), 400

    face_encodings = face_recognition.face_encodings(img, face_locations)
    if not face_encodings:
        return jsonify({"error": "Could not extract face encoding"}), 400

    target_encoding = face_encodings[0]

    # Compare against known encodings
    best_match = None
    best_distance = 1.0
    for entry in known_encodings:
        known_encoding = np.array(entry['encoding'])
        distance = face_recognition.face_distance([target_encoding], known_encoding)[0]
        if distance < best_distance:
            best_distance = distance
            best_match = entry['user_id']

    # Threshold: if distance > 0.6, treat as unknown
    if best_match is not None and best_distance < 0.6:
        return jsonify({"user_id": best_match, "distance": best_distance})
    else:
        return jsonify({"error": "No match found"}), 404

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5001, debug=True)
